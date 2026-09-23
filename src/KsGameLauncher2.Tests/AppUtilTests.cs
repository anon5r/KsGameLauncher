using System.IO;
using System.Windows.Forms;
using KsGameLauncher2.Utils;
using Xunit;

namespace KsGameLauncher2.Tests
{
    /// <summary>
    /// appinfo.json の保存先解決のテスト。
    /// 以前は読み込み側が実行ファイルの隣を見るのに対し、保存側が設定値のファイル名を
    /// そのまま File API に渡してカレントディレクトリへ書いていたため、作業ディレクトリが
    /// 実行ファイルの場所と異なる場合(カスタムURI konaste.*://launch/... 経由の起動など)に
    /// ダウンロードしたファイルを読み戻せず、FileInfo.Length が FileNotFoundException を
    /// 投げて async void の LoadGamesMenu 経由でクラッシュしていた。
    /// </summary>
    public class AppUtilTests
    {
        [Fact]
        public void GetAppInfoLocalPath_IsRootedNextToTheExecutable()
        {
            string path = AppUtil.GetAppInfoLocalPath();

            Assert.True(Path.IsPathRooted(path), $"expected an absolute path but got '{path}'");
            Assert.Equal(
                Path.GetDirectoryName(Application.ExecutablePath),
                Path.GetDirectoryName(path));
        }

        [Fact]
        public void GetAppInfoLocalPath_UsesConfiguredFileName()
        {
            string path = AppUtil.GetAppInfoLocalPath();

            Assert.Equal(Properties.Settings.Default.appInfoLocal, Path.GetFileName(path));
        }

        /// <summary>
        /// 回帰テストの本体: 作業ディレクトリを変更しても解決結果が変わらないこと。
        /// </summary>
        [Fact]
        public void GetAppInfoLocalPath_DoesNotDependOnCurrentDirectory()
        {
            string original = Directory.GetCurrentDirectory();
            string before = AppUtil.GetAppInfoLocalPath();
            try
            {
                Directory.SetCurrentDirectory(Path.GetTempPath());

                string after = AppUtil.GetAppInfoLocalPath();

                Assert.Equal(before, after);
                Assert.NotEqual(
                    Path.GetFullPath(Properties.Settings.Default.appInfoLocal),
                    after);
            }
            finally
            {
                Directory.SetCurrentDirectory(original);
            }
        }
    }
}
