using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using KsGameLauncher2.Properties;
using Xunit;

namespace KsGameLauncher2.Tests
{
    /// <summary>
    /// タスクトレイに常駐し、インストール済みゲームのメニューを表示する機能の回帰テスト。
    /// 実際の Konaste へのログイン・起動 (外部サービス通信) は行わず、そこに至る前段の
    /// メニュー構築ロジックのみを検証する。
    /// </summary>
    public class MainContextTests
    {
        /// <summary>
        /// コンストラクターを経由せずに MainContext のインスタンスを生成する。
        /// 通常のコンストラクターは NotifyIcon の生成や appinfo.json のダウンロードなど
        /// 外部リソースに触れる処理を伴うため、メニュー構築ロジックだけを単体テストするために使用する。
        /// </summary>
        private static MainContext CreateUninitialized()
        {
            return (MainContext)RuntimeHelpers.GetUninitializedObject(typeof(MainContext));
        }

        private static object? InvokePrivate(object target, string methodName, Type[]? paramTypes = null, object?[]? args = null)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo? method = paramTypes == null
                ? target.GetType().GetMethod(methodName, flags)
                : target.GetType().GetMethod(methodName, flags, binder: null, types: paramTypes, modifiers: null);

            Assert.True(method != null, $"Method '{methodName}' was not found by reflection.");
            return method!.Invoke(target, args);
        }

        /// <summary>
        /// 起動時に発生していた NullReferenceException の回帰テスト。
        /// InitializeComponent() が呼ばれていなかったため、デザイナーが生成するメニュー項目フィールド
        /// (aboutToolStripMenuItem 等) が null のままアクセスされ例外になっていた。
        /// </summary>
        [StaFact]
        public void CreateMinimalMenuStripItems_NormalMode_BuildsFullContextMenu()
        {
            MainContext context = CreateUninitialized();
            InvokePrivate(context, "InitializeComponent");

            var menu = (NotifyIconContextMenuStrip)InvokePrivate(context, "CreateMinimalMenuStripItems", Type.EmptyTypes, Array.Empty<object>())!;

            Assert.NotNull(menu);
            Assert.Equal(6, menu.Items.Count);
            Assert.Contains(menu.Items.Cast<ToolStripItem>(), i => i.Text == Strings.ContextMenuItems_About);
            Assert.Contains(menu.Items.Cast<ToolStripItem>(), i => i.Text == Strings.ContextMenuItems_Options);
            Assert.Contains(menu.Items.Cast<ToolStripItem>(), i => i.Text == Strings.ContextMenuItems_ManageAccount);
            Assert.Contains(menu.Items.Cast<ToolStripItem>(), i => i.Text == Strings.ContextMenuItems_AddNewGame);
            Assert.Contains(menu.Items.Cast<ToolStripItem>(), i => i.Text == Strings.ContextMenuItems_Exit);
        }

        /// <summary>
        /// バックグラウンド実行モード (RunBackground) では「終了」のみのメニューになる。
        /// こちらは InitializeComponent 未呼び出しでも動作する独立した分岐。
        /// </summary>
        [StaFact]
        public void CreateMinimalMenuStripItems_ExitOnlyMode_BuildsSingleExitItem()
        {
            MainContext context = CreateUninitialized();

            var menu = (NotifyIconContextMenuStrip)InvokePrivate(context, "CreateMinimalMenuStripItems", new[] { typeof(bool) }, new object[] { true })!;

            Assert.NotNull(menu);
            ToolStripItem item = Assert.Single(menu.Items.Cast<ToolStripItem>());
            Assert.Equal(Strings.ContextMenuItems_Exit, item.Text);
        }

        /// <summary>
        /// 「インストール済みのゲームのみ表示」設定時、コナステ未サブスクリプション等でゲームが
        /// 1つもインストールされていない場合はプレースホルダー項目にフォールバックすることを確認する。
        /// </summary>
        [StaFact]
        public void InitGameMenu_ShowOnlyInstalledGames_FallsBackToPlaceholder_WhenNoGamesInstalled()
        {
            bool original = Settings.Default.ShowOnlyInstalledGames;
            try
            {
                Settings.Default.ShowOnlyInstalledGames = true;
                AppInfo.LoadFromJson(SampleAppInfoJson);

                MainContext context = CreateUninitialized();
                var menu = (NotifyIconContextMenuStrip)InvokePrivate(context, "InitGameMenu")!;

                ToolStripItem item = Assert.Single(menu.Items.Cast<ToolStripItem>());
                Assert.Equal(Strings.NoInstalledGames, item.Text);
                Assert.False(item.Enabled);
            }
            finally
            {
                Settings.Default.ShowOnlyInstalledGames = original;
                AppInfo.LoadFromJson("[]");
            }
        }

        /// <summary>
        /// 「インストール済みのゲームのみ表示」を無効にした場合は、未インストールのゲームも
        /// 無効化した状態でメニューに列挙されることを確認する。
        /// </summary>
        [StaFact]
        public void InitGameMenu_ShowAllGames_ListsDisabledEntries_ForUninstalledGames()
        {
            bool original = Settings.Default.ShowOnlyInstalledGames;
            try
            {
                Settings.Default.ShowOnlyInstalledGames = false;
                AppInfo.LoadFromJson(SampleAppInfoJson);

                MainContext context = CreateUninitialized();
                var menu = (NotifyIconContextMenuStrip)InvokePrivate(context, "InitGameMenu")!;

                Assert.Equal(2, menu.Items.Count);
                Assert.All(menu.Items.Cast<ToolStripItem>(), i => Assert.False(i.Enabled));
            }
            finally
            {
                Settings.Default.ShowOnlyInstalledGames = original;
                AppInfo.LoadFromJson("[]");
            }
        }

        private const string SampleAppInfoJson = @"
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
    }
}
