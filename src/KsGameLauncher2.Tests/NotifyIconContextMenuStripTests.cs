using System.Windows.Forms;
using Xunit;

namespace KsGameLauncher2.Tests
{
    public class NotifyIconContextMenuStripTests
    {
        /// <summary>
        /// 起動時に発生していた「Failed to get window fields by reflection」の回帰テスト。
        /// .NET 10 では NotifyIcon の内部フィールド名が "window" から "_window" に変更されたため、
        /// 実際の NotifyIcon インスタンスに対してリフレクションでハンドルを取得できることを確認する。
        /// </summary>
        [StaFact]
        public void GetNotifyIconWindowHandle_ReturnsValidHandle_ForRealNotifyIcon()
        {
            using var notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                // 実際のバグはタスクトレイに表示された状態でクリックした際に発生するため、
                // ネイティブウィンドウが生成されるよう Visible を true にする。
                Visible = true,
            };

            var handle = NotifyIconContextMenuStrip.GetNotifyIconWindowHandle(notifyIcon);

            Assert.NotEqual(IntPtr.Zero, handle);
        }

        [StaFact]
        public void GetNotifyIconWindowHandle_DoesNotThrow_ForRealNotifyIcon()
        {
            using var notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = false,
            };

            var exception = Record.Exception(() => NotifyIconContextMenuStrip.GetNotifyIconWindowHandle(notifyIcon));

            Assert.Null(exception);
        }
    }
}
