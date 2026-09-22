using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace KsGameLauncher2
{
    internal class NotifyIconContextMenuStrip : ContextMenuStrip
    {

        public NotifyIconContextMenuStrip() : base()
        {
        }
        public NotifyIconContextMenuStrip(IContainer container) : base(container)
        {
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Focused) this.Focus();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEACTIVATE = 0x0021;
            if (m.Msg != WM_MOUSEACTIVATE)
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// 既知のフィールド名候補。.NET のバージョンによって NotifyIcon の内部フィールド名が
        /// "window" から "_window" に変更されているため、両方を試す。
        /// </summary>
        /// <see cref="https://tan.hatenadiary.jp/entry/2020/10/16/204405"/>
        /// <param name="control">System.Windows.Forms.Control</param>
        /// <param name="point">System.Drawing.Point</param>
        /// <param name="notifyIcon">NOtifyIcon</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void ShowContext(Control control, Point point, NotifyIcon notifyIcon)
        {
            SetForegroundWindow(GetNotifyIconWindowHandle(notifyIcon));
            Show(control, point);
        }

        private static readonly string[] KnownNativeWindowFieldNames = { "_window", "window" };

        /// <summary>
        /// NotifyIcon が内部に保持する NativeWindow の Handle をリフレクションで取得する。
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal static IntPtr GetNotifyIconWindowHandle(NotifyIcon notifyIcon)
        {
            var fiWindow = FindNativeWindowField(notifyIcon.GetType());
            if (fiWindow == null) { throw new InvalidOperationException("Failed to get window fields by reflection"); }

            var notifyIconNativeWindow = fiWindow.GetValue(notifyIcon);
            if (notifyIconNativeWindow == null) { throw new InvalidOperationException("Failed to get window fields by reflection"); }

            // 現状はHandleはpublicだが、internal等になってもいいようにBindingFlagsを追加
            var piHandle = notifyIconNativeWindow.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (piHandle == null) { throw new InvalidOperationException("Failed to get handled properties by reflection"); }

            return (IntPtr)piHandle.GetValue(notifyIconNativeWindow)!;
        }

        private static FieldInfo? FindNativeWindowField(Type notifyIconType)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var name in KnownNativeWindowFieldNames)
            {
                var field = notifyIconType.GetField(name, flags);
                if (field != null) { return field; }
            }

            // フィールド名が既知の候補と異なる場合に備え、型名で探索する
            foreach (var field in notifyIconType.GetFields(flags))
            {
                if (field.FieldType.Name.Contains("NativeWindow", StringComparison.Ordinal))
                {
                    return field;
                }
            }

            return null;
        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
