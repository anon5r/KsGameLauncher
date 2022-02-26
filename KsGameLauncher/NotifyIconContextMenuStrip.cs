using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KsGameLauncher
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
        /// 
        /// </summary>
        /// <see cref="https://tan.hatenadiary.jp/entry/2020/10/16/204405"/>
        /// <param name="control">System.Windows.Forms.Control</param>
        /// <param name="point">System.Drawing.Point</param>
        /// <param name="notifyIcon">NOtifyIcon</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void ShowContext(Control control, Point point, NotifyIcon notifyIcon)
        {
            var fiWindow = notifyIcon.GetType().GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fiWindow == null) { throw new InvalidOperationException("Failed to get window fields by reflection"); }

            var notifyIconNativeWindow = fiWindow.GetValue(notifyIcon);
            // 現状はHandleはpublicだが、internal等になってもいいようにBindingFlagsを追加
            var piHandle = notifyIconNativeWindow.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (piHandle == null) { throw new InvalidOperationException("Failed to get handled properties by reflection"); }
            SetForegroundWindow((IntPtr)piHandle.GetValue(notifyIconNativeWindow));
            Show(control, point);

        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
