using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Web.WebView2.Core;

namespace KsGameLauncher2.Web
{
    /// <summary>
    /// 起動ページを開き、ゲーム起動用のカスタム URI (konaste.xxx://login...) を取得する。
    /// CoreWebView2 にだけ依存し、WinForms / WPF のどちらのホストからも使える。
    /// </summary>
    internal sealed class WebLaunchSession : IDisposable
    {
        private const int ProbeAttempts = 10;
        private static readonly TimeSpan ProbeInterval = TimeSpan.FromMilliseconds(300);
        // 規約同意などの寄り道のあと、起動ページへ戻る回数の上限 (無限ループ防止)
        private const int MaxReturnNavigations = 3;

        private readonly CoreWebView2 _web;
        private readonly AppInfo _app;
        private readonly Uri _launchPage;
        private readonly string _loginPath;
        private readonly string _privacyPath;
        private readonly TaskCompletionSource<string> _result = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private bool _detoured;
        private int _returnCount;
        private int _probeGeneration;
        private bool _disposed;

        /// <summary>ユーザーの操作が必要になった (ログイン、規約同意、起動ボタンが見つからない等)。</summary>
        public event EventHandler? RevealRequested;

        public WebLaunchSession(CoreWebView2 web, AppInfo app)
        {
            _web = web;
            _app = app;
            _launchPage = app.Launch.GetUri();
            _loginPath = new Uri(Properties.Settings.Default.LoginURL).AbsolutePath;
            _privacyPath = new Uri(Properties.Settings.Default.PrivacyConfirmPath).AbsolutePath;

            _web.NavigationStarting += OnNavigationStarting;
            _web.NavigationCompleted += OnNavigationCompleted;
            _web.LaunchingExternalUriScheme += OnLaunchingExternalUriScheme;
            _web.NewWindowRequested += OnNewWindowRequested;
        }

        /// <summary>起動用 URI を取得するまで待つ。キャンセルされた場合は OperationCanceledException。</summary>
        public async Task<string> RunAsync(CancellationToken cancellationToken)
        {
            using var registration = cancellationToken.Register(() => _result.TrySetCanceled(cancellationToken));
            _web.Navigate(_launchPage.AbsoluteUri);
            return await _result.Task;
        }

        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
#if DEBUG
            // クエリには認可コードなどが含まれるため出力しない
            Debug.WriteLine($"[WebLaunch] navigate: {StripQuery(e.Uri)}");
#endif
        }

        private async void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_disposed || _result.Task.IsCompleted)
                return;

            try
            {
                await HandleNavigationCompletedAsync(e);
            }
            catch (Exception ex)
            {
                _result.TrySetException(ex);
            }
        }

        private async Task HandleNavigationCompletedAsync(CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                if (e.WebErrorStatus == CoreWebView2WebErrorStatus.OperationCanceled)
                    return; // 次の遷移で上書きされただけ
                Debug.WriteLine($"[WebLaunch] navigation failed: {e.WebErrorStatus}");
                RequestReveal(); // エラーページをユーザーに見せる
                return;
            }

            if (!Uri.TryCreate(_web.Source, UriKind.Absolute, out Uri? current))
                return;

            PageKind kind = LaunchPageClassifier.Classify(current, _launchPage, _loginPath,
                Properties.Resources.TosCheckPath, _privacyPath);
            Debug.WriteLine($"[WebLaunch] page: {kind}");

            switch (kind)
            {
                case PageKind.Login:
                case PageKind.TermsOfService:
                case PageKind.PrivacyConfirmation:
                    // 旧実装では外部ブラウザを開いていたが、同じウィンドウ内で済ませられる
                    _detoured = true;
                    RequestReveal();
                    break;

                case PageKind.LaunchPage:
                    await ProbeLaunchPageAsync();
                    break;

                default:
                    if (_detoured && _returnCount < MaxReturnNavigations)
                    {
                        // ログインや規約同意を終えて別ページに着地した場合は、起動ページへ戻す
                        _detoured = false;
                        _returnCount++;
                        _web.Navigate(_launchPage.AbsoluteUri);
                    }
                    else
                    {
                        RequestReveal();
                    }
                    break;
            }
        }

        /// <summary>起動ボタンの href とメンテナンス表示を DOM から読み取る。クリックはしない。</summary>
        private async Task ProbeLaunchPageAsync()
        {
            int generation = ++_probeGeneration;
            string script =
                "(() => {" +
                $" const a = document.querySelector({JsonSerializer.Serialize(_app.Launch.Selector)});" +
                $" const m = {JsonSerializer.Serialize(Properties.Resources.MaintenanceCheckString)};" +
                " return { href: a ? a.getAttribute('href') : null," +
                "          maintenance: !!document.body && document.body.innerText.includes(m) };" +
                "})()";

            // ボタンが JS で後から描画される場合に備えて数回試す
            for (int i = 0; i < ProbeAttempts; i++)
            {
                if (_disposed || _result.Task.IsCompleted || generation != _probeGeneration)
                    return;

                string json = await _web.ExecuteScriptAsync(script);
                LaunchPageProbe? probe = JsonSerializer.Deserialize<LaunchPageProbe>(json);

                if (probe?.Maintenance == true)
                    throw new LauncherException(Properties.Strings.UnderMaintenanceMessage, _web.Source);

                string? href = probe?.Href;
                if (LaunchPageClassifier.IsGameLaunchUri(href))
                {
                    Debug.WriteLine("[WebLaunch] launch uri found in DOM");
                    _result.TrySetResult(href);
                    return;
                }

                await Task.Delay(ProbeInterval);
            }

            // セレクタが古くなっている可能性がある。ユーザーに手動でクリックしてもらい、
            // LaunchingExternalUriScheme で受け取る。
            Debug.WriteLine("[WebLaunch] launch button not found (selector may be outdated)");
            RequestReveal();
        }

        private void OnLaunchingExternalUriScheme(object? sender, CoreWebView2LaunchingExternalUriSchemeEventArgs e)
        {
            if (!LaunchPageClassifier.IsGameLaunchUri(e.Uri))
                return;

            // 既定の確認ダイアログを出さず、ランチャー側で起動する
            e.Cancel = true;
            Debug.WriteLine("[WebLaunch] launch uri captured from click");
            _result.TrySetResult(e.Uri);
        }

        private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // ポップアップも同じ WebView 内で開き、Cookie を共有させる。
            // ※ 認証フローが window.opener との postMessage に依存している場合は、
            //   別の WebView2 を作って e.NewWindow に割り当てる方式に変更する必要がある。
            e.Handled = true;
            _web.Navigate(e.Uri);
        }

        private void RequestReveal() => RevealRequested?.Invoke(this, EventArgs.Empty);

        private static string StripQuery(string uri)
        {
            int i = uri.IndexOfAny(['?', '#']);
            return i < 0 ? uri : uri[..i];
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _web.NavigationStarting -= OnNavigationStarting;
            _web.NavigationCompleted -= OnNavigationCompleted;
            _web.LaunchingExternalUriScheme -= OnLaunchingExternalUriScheme;
            _web.NewWindowRequested -= OnNewWindowRequested;
            _result.TrySetCanceled();
        }

        private sealed record LaunchPageProbe(
            [property: JsonPropertyName("href")] string? Href,
            [property: JsonPropertyName("maintenance")] bool Maintenance);
    }
}
