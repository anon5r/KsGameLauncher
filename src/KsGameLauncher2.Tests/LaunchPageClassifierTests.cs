using KsGameLauncher2.Web;
using Xunit;

namespace KsGameLauncher2.Tests
{
    public class LaunchPageClassifierTests
    {
        private static readonly Uri LaunchPage =
            new("https://p.eagate.573.jp/game/konasteapp/API/login/login.html?game_id=sdvx");

        private const string LoginPath = "/gate/p/login.html";
        private const string TosPath = "/terms_of_service/index.html";
        private const string PrivacyPath = "/gate/agree/p/privacy_confirm.html";

        private static PageKind Classify(string url)
            => LaunchPageClassifier.Classify(new Uri(url), LaunchPage, LoginPath, TosPath, PrivacyPath);

        [Theory]
        [InlineData("https://account.konami.net/auth/login.html")]
        [InlineData("https://some-new-idp.example.com/oauth/authorize?client_id=x")]
        public void OtherHost_IsLogin(string url)
            => Assert.Equal(PageKind.Login, Classify(url));

        [Fact]
        public void EamusementLoginPage_IsLogin()
            => Assert.Equal(PageKind.Login, Classify("https://p.eagate.573.jp/gate/p/login.html"));

        [Fact]
        public void LaunchPage_IgnoresQuery()
            => Assert.Equal(PageKind.LaunchPage,
                Classify("https://p.eagate.573.jp/game/konasteapp/API/login/login.html?game_id=sdvx&x=1"));

        [Fact]
        public void TermsOfService_IsDetected()
            => Assert.Equal(PageKind.TermsOfService,
                Classify("https://p.eagate.573.jp/game/sdvx/terms_of_service/index.html"));

        [Fact]
        public void PrivacyConfirmation_IsDetected()
            => Assert.Equal(PageKind.PrivacyConfirmation, Classify("https://p.eagate.573.jp" + PrivacyPath));

        [Fact]
        public void OtherEamusementPage_IsUnknown()
            => Assert.Equal(PageKind.Unknown, Classify("https://p.eagate.573.jp/game/konasteapp/"));

        [Theory]
        [InlineData("konaste.sdvx://login?tk=abc", true)]
        [InlineData("konaste.bomber-girl://login?tk=abc", true)]
        [InlineData("bm2dxinf://login?tk=abc", true)]
        [InlineData("konaste.sdvx://other", false)]
        [InlineData("https://p.eagate.573.jp/", false)]
        [InlineData("javascript:alert(1)", false)]
        [InlineData(null, false)]
        public void IsGameLaunchUri(string? uri, bool expected)
            => Assert.Equal(expected, LaunchPageClassifier.IsGameLaunchUri(uri));
    }
}
