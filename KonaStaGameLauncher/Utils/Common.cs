using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace KonaStaGameLauncher.Utils
{
    internal class Common
    {

        /// <summary>
        /// Check running with Administrator
        /// </summary>
        /// <returns>True = Administrator, False = General user</returns>
        public static bool isAdmin()
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
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Verb = "runas";

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
    }
}
