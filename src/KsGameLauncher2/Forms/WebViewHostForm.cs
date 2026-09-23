using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace KsGameLauncher2.Forms
{
    /// <summary>
    /// WebView2 を載せる WinForms 側の薄いホスト。最初は透明のまま処理し、ユーザー操作が必要な時だけ表示する。
    /// WPF へ移行する際は、このクラスだけを書き直せばよい (Web/ 以下はそのまま使える)。
    /// UI スレッド (Application.Run のメッセージループ上) から呼び出すこと。
    /// </summary>
    internal sealed class WebViewHostForm : Form
    {
        // この時間内に終わらなければ、状況をユーザーに見せる (中断はしない)
        private static readonly TimeSpan HiddenTimeout = TimeSpan.FromSeconds(20);

        private static Task<CoreWebView2Environment>? _environment;

        private readonly WebView2 _webView = new() { Dock = DockStyle.Fill };
        private readonly System.Windows.Forms.Timer _revealTimer = new();
        private readonly CancellationTokenSource _userClosed = new();
        private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly Func<WebViewHostForm, CoreWebView2, Task> _work;
        private bool _finished;
        private bool _closedByUser;

        /// <summary>ユーザーがウィンドウを閉じたらキャンセルされる。</summary>
        internal CancellationToken UserClosed => _userClosed.Token;

        private WebViewHostForm(string title, Func<WebViewHostForm, CoreWebView2, Task> work)
        {
            _work = work;

            Text = title;
            Icon = Properties.Resources.appIcon;
            ClientSize = new Size(520, 760);
            StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(_webView);

            // WebView2 の初期化にはウィンドウハンドルが必要なので、Hide ではなく透明にしておく
            Opacity = 0;
            ShowInTaskbar = false;

            _revealTimer.Interval = (int)HiddenTimeout.TotalMilliseconds;
            _revealTimer.Tick += (_, _) => Reveal();
        }

        /// <summary>WebView2 を用意して work を実行し、終わったらウィンドウを閉じる。</summary>
        internal static Task RunAsync(string title, Func<WebViewHostForm, CoreWebView2, Task> work)
        {
            var form = new WebViewHostForm(title, work);
            form.Show();
            return form._completion.Task;
        }

        /// <summary>
        /// 全ウィンドウで共有する環境。UserDataFolder を明示しないと exe の隣に作られるため、
        /// Program Files 等に置かれた場合に書き込めず失敗する。
        /// </summary>
        private static Task<CoreWebView2Environment> GetEnvironmentAsync()
        {
            return _environment ??= CoreWebView2Environment.CreateAsync(
                browserExecutableFolder: null,
                userDataFolder: Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "KsGameLauncher", "WebView2"));
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _revealTimer.Start();

            try
            {
                await _webView.EnsureCoreWebView2Async(await GetEnvironmentAsync());

                // WebView2 の既定はどちらも無効。ログインのたびに手入力させないため、
                // ブラウザと同じようにパスワードの保存と自動入力を有効にする。
                // 保存先は UserDataFolder (%LocalAppData%\KsGameLauncher\WebView2) で、
                // Windows のユーザーアカウントに紐づいて暗号化される。
                _webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
                _webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = true;

                await _work(this, _webView.CoreWebView2);
                _completion.TrySetResult();
            }
            catch (Exception ex) when (ex is OperationCanceledException || _closedByUser)
            {
                // ユーザーが閉じた場合、破棄済みの WebView2 から別の例外が出ることがあるので、まとめてキャンセル扱いにする
                _completion.TrySetCanceled();
            }
            catch (Exception ex)
            {
                _completion.TrySetException(ex);
            }

            _finished = true;
            if (!IsDisposed)
                Close();
        }

        /// <summary>ウィンドウを表示してユーザーの操作を待つ。</summary>
        internal void Reveal()
        {
            _revealTimer.Stop();
            if (IsDisposed || Opacity > 0)
                return;

            Opacity = 1;
            ShowInTaskbar = true;
            Activate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_finished)
            {
                _closedByUser = true;
                _userClosed.Cancel(); // ユーザーによる中断
            }
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _revealTimer.Dispose();
                _userClosed.Dispose();
                _webView.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
