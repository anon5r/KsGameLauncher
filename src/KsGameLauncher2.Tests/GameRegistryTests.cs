using KsGameLauncher2.Utils;
using Xunit;

namespace KsGameLauncher2.Tests
{
    /// <summary>
    /// インストール済みゲームの検出 (レジストリ HKLM\SOFTWARE\KONAMI\{name}) のテスト。
    /// テスト実行環境にはコナステのゲームはインストールされていない前提 (サブスクリプション未加入) のため、
    /// 「未インストール」として安全に扱われることを検証する。
    /// </summary>
    public class GameRegistryTests
    {
        private const string NonExistentGameName = "KsGameLauncherTests_NonExistentGame";

        [Fact]
        public void IsInstalled_ReturnsFalse_ForGameNotPresentInRegistry()
        {
            Assert.False(GameRegistry.IsInstalled(NonExistentGameName));
        }

        [Fact]
        public void GetInstallDir_ReturnsNull_ForGameNotPresentInRegistry()
        {
            Assert.Null(GameRegistry.GetInstallDir(NonExistentGameName));
        }

        [Fact]
        public void GetResourceDir_ReturnsNull_ForGameNotPresentInRegistry()
        {
            Assert.Null(GameRegistry.GetResourceDir(NonExistentGameName));
        }

        [Fact]
        public void GetInstallDir_ReturnsNull_ForNullOrEmptyGameName()
        {
            Assert.Null(GameRegistry.GetInstallDir(null!));
            Assert.Null(GameRegistry.GetInstallDir(""));
        }

        /// <summary>
        /// 未インストールのゲームではスキームのキー自体が存在しない。
        /// 以前は null を返した RegistryKey をそのまま参照して NullReferenceException になっていた。
        /// WebLauncher はゲーム起動直前にこれを呼ぶため、未インストール時に例外ではなく
        /// null を返してランチャー未インストールとして扱えることが必要。
        /// </summary>
        [Fact]
        public void GetLauncherPath_ReturnsNull_ForUnregisteredScheme()
        {
            Assert.Null(GameRegistry.GetLauncherPath("konaste.ksgamelauncher-tests-not-registered"));
        }

        [Fact]
        public void GetLauncherPath_ReturnsNull_ForNullOrEmptyScheme()
        {
            Assert.Null(GameRegistry.GetLauncherPath(null!));
            Assert.Null(GameRegistry.GetLauncherPath(""));
        }
    }
}
