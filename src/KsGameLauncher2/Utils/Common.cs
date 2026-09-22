using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace KsGameLauncher2.Utils
{
    internal class Common
    {

        /// <summary>
        /// Check running with Administrator
        /// </summary>
        /// <returns>True = Administrator, False = General user</returns>
        public static bool IsAdmin()
        {
            WindowsIdentity usrId = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(usrId);
            return p.IsInRole(@"BUILTIN\Administrators");
        }

        /// <summary>
        /// Run application with Administrator
        /// </summary>
        public static void RestartApplicationAtAdministratorAuthority()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Application.ExecutablePath,
                Verb = "runas"
            };

            try
            {
                Process p = Process.Start(startInfo);
            }
            catch
            {
                return;
            }
            Application.Exit();
        }

        public static void OpenControlPanel()
        {
            ProcessStartInfo info = new ProcessStartInfo("rundll32.exe")
            {
                Arguments = "shell32.dll,Control_RunDLL"
            };
            Process proc = new Process
            {
                StartInfo = info
            };
            proc.Start();
        }

        public static void OpenControlPanel(string name)
        {
            ProcessStartInfo info = new ProcessStartInfo("rundll32.exe")
            {
                Arguments = "shell32.dll,Control_RunDLL " + name
            };
            Process proc = new Process
            {
                StartInfo = info
            };
            proc.Start();
        }

        /// <summary>
        /// .NET (Core以降) は ProcessStartInfo.UseShellExecute の既定値が false のため、
        /// 明示的に true にしないと URL を実行ファイルパスとして解釈しようとして失敗する。
        /// </summary>
        internal static ProcessStartInfo CreateUrlProcessStartInfo(string url)
        {
            return new ProcessStartInfo(url) { UseShellExecute = true };
        }

        public static void OpenUrlByDefaultBrowser(string url)
        {
            Process.Start(CreateUrlProcessStartInfo(url));
        }

        public static void OpenUrlByDefaultBrowser(Uri uri)
        {
            OpenUrlByDefaultBrowser(uri.AbsoluteUri);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
