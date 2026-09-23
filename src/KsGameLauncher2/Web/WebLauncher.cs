using System.Diagnostics;
using KsGameLauncher2.Forms;
using Microsoft.Web.WebView2.Core;

namespace KsGameLauncher2.Web
{
    /// <summary>
    /// Launcher.StartApp / Launcher.Logout の WebView2 版。
    /// ID・パスワードの保存、HttpClient によるフォーム POST、OTP ダイアログは不要になる。
    /// UI スレッドから呼び出すこと。
    /// </summary>
    internal static class WebLauncher
    {
        private static readonly TimeSpan LogoutTimeout = TimeSpan.FromSeconds(10);

        /// <exception cref="LoginCancelException">ユーザーがウィンドウを閉じた</exception>
        /// <exception cref="LauncherException">メンテナンス中、ランチャー未インストールなど</exception>
        internal static async Task StartAppAsync(AppInfo app)
        {
            string? launchUri = null;
            try
            {
                await WebViewHostForm.RunAsync(app.Name, async (host, web) =>
                {
                    using var session = new WebLaunchSession(web, app);
                    session.RevealRequested += (_, _) => host.Reveal();
                    launchUri = await session.RunAsync(host.UserClosed);
                });
            }
            catch (OperationCanceledException)
            {
                // 既存の呼び出し元 (MainContext / Launcher.LaunchGames) はこの例外をキャンセルとして扱っている
                throw new LoginCancelException();
            }
            catch (WebView2RuntimeNotFoundException)
            {
                // TODO: Strings.resx に文言を追加する
                throw new LauncherException("Microsoft Edge WebView2 Runtime is not installed.",
                    "https://developer.microsoft.com/microsoft-edge/webview2/");
            }

            if (launchUri == null)
                return;

            if (Properties.Settings.Default.EnableNotification && Program.mainContext != null)
            {
                Program.mainContext.DisplayToolTip(
                    string.Format(Properties.Strings.IconBalloonMessage_Launching, app.Name),
                    Properties.Settings.Default.NotificationTimeout);
            }

            StartGameLauncher(launchUri);
        }

        /// <summary>旧 Launcher.LauncherLoginPage の起動部分と同じ方法でランチャーを起動する。</summary>
        internal static void StartGameLauncher(string launchUri)
        {
            if (!LaunchPageClassifier.IsGameLaunchUri(launchUri))
                throw new LauncherException("Unexpected launch URI.");

            string scheme = new Uri(launchUri).Scheme;
            string? launcherPath = Utils.GameRegistry.GetLauncherPath(scheme);
            if (string.IsNullOrEmpty(launcherPath))
                throw new LauncherException($"Launcher for \"{scheme}\" is not installed.");

            Process.Start(launcherPath, launchUri);
        }

        /// <summary>サーバー側のセッションを破棄したうえで、WebView2 の Cookie を削除する。</summary>
        internal static Task LogoutAsync()
        {
            return WebViewHostForm.RunAsync(Properties.Strings.AppName, async (host, web) =>
            {
                var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                void OnCompleted(object? s, CoreWebView2NavigationCompletedEventArgs e) => completed.TrySetResult();

                web.NavigationCompleted += OnCompleted;
                try
                {
                    web.Navigate(Properties.Settings.Default.LogoutURL);
                    await Task.WhenAny(completed.Task, Task.Delay(LogoutTimeout));
                }
                finally
                {
                    web.NavigationCompleted -= OnCompleted;
                    web.CookieManager.DeleteAllCookies();
                }
            });
        }
    }
}
