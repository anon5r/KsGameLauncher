using System.Text;
using Xunit;

namespace KsGameLauncher2.Tests
{
    /// <summary>
    /// Konaste への実際のログイン・起動処理はネットワーク通信を伴うため対象外とし、
    /// そこに至るまでのロジック (文字コード判定、ログイン状態判定) のみを検証する。
    /// </summary>
    public class LauncherTests
    {
        [Theory]
        [InlineData("sjis")]
        [InlineData("s-jis")]
        [InlineData("windows-31j")]
        [InlineData("cp932")]
        [InlineData("ms932")]
        [InlineData("SJIS")]
        public void EncodingMapJapanese_KnownShiftJisAliases_ResolveToShiftJis(string alias)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Encoding encoding = Launcher.EncodingMapJapanese(alias);

            Assert.Equal(Encoding.GetEncoding("shift-jis").CodePage, encoding.CodePage);
        }

        [Fact]
        public void EncodingMapJapanese_UnknownEncodingName_FallsBackToUtf8()
        {
            Encoding encoding = Launcher.EncodingMapJapanese("this-encoding-does-not-exist");

            Assert.Equal(Encoding.UTF8.CodePage, encoding.CodePage);
        }

        [Fact]
        public void EncodingMapJapanese_KnownIanaName_ResolvesToThatEncoding()
        {
            Encoding encoding = Launcher.EncodingMapJapanese("utf-8");

            Assert.Equal(Encoding.UTF8.CodePage, encoding.CodePage);
        }

        /// <summary>
        /// ログイン前 (Cookie 未保存) の状態では IsLogin() がネットワーク通信を行わずに
        /// false を返すことを確認する。サブスクリプション未加入で実ログインができない環境でも
        /// ここまでは実行可能であるべき。
        /// </summary>
        [Fact]
        public void IsLogin_WithoutSavedCookies_ReturnsFalseWithoutNetworkAccess()
        {
            Properties.Settings.Default.Cookies = null;

            Launcher launcher = Launcher.Create();

            Assert.False(launcher.IsLogin());
        }
    }
}
