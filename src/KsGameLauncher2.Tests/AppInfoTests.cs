using Xunit;

namespace KsGameLauncher2.Tests
{
    /// <summary>
    /// サーバーから配信される appinfo.json (ゲーム一覧・起動情報) のパース処理のテスト。
    /// タスクトレイメニューはこの情報を元に構築される。
    /// </summary>
    public class AppInfoTests
    {
        private const string ValidJson = @"
        [
          {
            ""game_id"": ""sdvx"",
            ""name"": ""SOUND VOLTEX"",
            ""launch"": { ""url"": ""https://example.invalid/launch/sdvx"", ""selector"": ""#launch"" },
            ""iconFile"": ""{{installDir}}\\icon.ico""
          },
          {
            ""game_id"": ""iidx"",
            ""name"": ""beatmania IIDX"",
            ""launch"": { ""url"": ""https://example.invalid/launch/iidx"", ""selector"": ""#launch"" },
            ""iconFile"": ""{{installDir}}\\icon.ico""
          }
        ]";

        [Fact]
        public void LoadFromJson_ValidJson_PopulatesList()
        {
            var list = AppInfo.LoadFromJson(ValidJson);

            Assert.NotNull(list);
            Assert.Equal(2, list.Count);
            Assert.Equal("sdvx", list[0].ID);
            Assert.Equal("SOUND VOLTEX", list[0].Name);
            Assert.Equal("https://example.invalid/launch/sdvx", list[0].Launch.URL);
        }

        [Fact]
        public void LoadFromJson_InvalidJson_ReturnsNull()
        {
            var list = AppInfo.LoadFromJson("{ not valid json ");

            Assert.Null(list);
        }

        [Fact]
        public void ContainID_And_Find_ReflectLoadedList()
        {
            AppInfo.LoadFromJson(ValidJson);

            Assert.True(AppInfo.ContainID("sdvx"));
            Assert.False(AppInfo.ContainID("unknown-game"));

            AppInfo found = AppInfo.Find("iidx");
            Assert.NotNull(found);
            Assert.Equal("beatmania IIDX", found.Name);

            Assert.Null(AppInfo.Find("unknown-game"));
        }

        /// <summary>
        /// ゲーム未インストール (レジストリにキーが無い) 環境で iconFile が {{installDir}} を
        /// 含んでいても例外にならず、既定のアイコンにフォールバックすることを確認する。
        /// サブスクリプション未加入・ゲーム未インストールの実行環境を想定した回帰テスト。
        /// </summary>
        [StaFact]
        public void GetIcon_FallsBackToDefaultIcon_WhenGameIsNotInstalled()
        {
            var list = AppInfo.LoadFromJson(ValidJson);
            AppInfo app = list[0];

            var icon = app.GetIcon();

            Assert.NotNull(icon);
        }
    }
}
