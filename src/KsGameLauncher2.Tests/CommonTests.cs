using System.Diagnostics;
using KsGameLauncher2.Utils;
using Xunit;

namespace KsGameLauncher2.Tests
{
    /// <summary>
    /// 「指定されたファイルが見つかりません」(Win32Exception) の回帰テスト。
    /// .NET (Core以降) は ProcessStartInfo.UseShellExecute の既定値が .NET Framework 時代の true から
    /// false に変わったため、URL をそのまま Process.Start(string) に渡すと実行ファイルパスとして
    /// 解釈されて失敗していた。実際にブラウザを起動せず、UseShellExecute が true で構築されることのみ検証する。
    /// </summary>
    public class CommonTests
    {
        [Theory]
        [InlineData("https://my.konami.net/otp.html")]
        [InlineData("https://github.com/anon5r/KsGameLauncher")]
        public void CreateUrlProcessStartInfo_UsesShellExecute(string url)
        {
            ProcessStartInfo startInfo = Common.CreateUrlProcessStartInfo(url);

            Assert.True(startInfo.UseShellExecute);
            Assert.Equal(url, startInfo.FileName);
        }
    }
}
