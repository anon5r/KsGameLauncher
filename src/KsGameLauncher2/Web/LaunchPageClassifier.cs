using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace KsGameLauncher2.Web
{
    internal enum PageKind
    {
        /// <summary>判定できないページ (e-amusement 内の別ページなど)</summary>
        Unknown,
        /// <summary>ログイン / OAuth 認可フロー中</summary>
        Login,
        /// <summary>ゲーム利用規約の同意ページ</summary>
        TermsOfService,
        /// <summary>プライバシーポリシーの確認ページ</summary>
        PrivacyConfirmation,
        /// <summary>appinfo.json の launch.url で指定された起動ページ</summary>
        LaunchPage,
    }

    /// <summary>
    /// WebView2 の遷移先 URL を分類する。WebView2 に依存しない純粋なロジックなので単体テストできる。
    /// </summary>
    internal static partial class LaunchPageClassifier
    {
        // 旧 Launcher.LauncherLoginPage と同じ条件 (konaste.xxx://login / bm2dxinf://login)
        [GeneratedRegex(@"^(konaste\.[a-z0-9\-]+|bm2dxinf)://login", RegexOptions.CultureInvariant)]
        private static partial Regex GameLaunchUriPattern();

        internal static bool IsGameLaunchUri([NotNullWhen(true)] string? uri)
            => uri != null && GameLaunchUriPattern().IsMatch(uri);

        /// <param name="current">現在表示しているページの URL</param>
        /// <param name="launchPage">起動ページの URL (AppInfo.Launch.URL)</param>
        /// <param name="loginPath">e-amusement 側のログインページのパス (Settings.LoginURL)</param>
        /// <param name="tosPath">利用規約ページのパス (Resources.TosCheckPath)</param>
        /// <param name="privacyPath">プライバシー確認ページのパス (Settings.PrivacyConfirmPath)</param>
        internal static PageKind Classify(Uri current, Uri launchPage, string loginPath, string tosPath, string privacyPath)
        {
            // 起動ページと別のホストに居る = 認証フロー中とみなす。
            // 認証ドメイン (旧 AuthorizeDomain) を固定で持たないので、OAuth 側のドメインや遷移が変わっても影響を受けない。
            if (!string.Equals(current.Host, launchPage.Host, StringComparison.OrdinalIgnoreCase))
                return PageKind.Login;

            string path = current.AbsolutePath;

            if (ContainsPath(path, loginPath))
                return PageKind.Login;
            if (ContainsPath(path, tosPath))
                return PageKind.TermsOfService;
            if (ContainsPath(path, privacyPath))
                return PageKind.PrivacyConfirmation;
            if (string.Equals(path, launchPage.AbsolutePath, StringComparison.OrdinalIgnoreCase))
                return PageKind.LaunchPage;

            return PageKind.Unknown;
        }

        private static bool ContainsPath(string path, string target)
            => !string.IsNullOrEmpty(target) && path.Contains(target, StringComparison.OrdinalIgnoreCase);
    }
}
