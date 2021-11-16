using System;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace KonaStaGameLauncher
{
    internal static class Program
    {

        private static System.Threading.Mutex _mutex;

        private static MainForm mainForm;

        /// <summary>
        /// Main entrypoint of an application
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Setup user language
            try
            {
                Properties.Resources.Culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
            } catch
            {
                Properties.Resources.Culture = new CultureInfo("en-US");
            }
            Properties.Resources.Culture = new CultureInfo("ja-JP");


            // Default displayed default messages on C#
            Thread.CurrentThread.CurrentCulture = Properties.Resources.Culture;
            Thread.CurrentThread.CurrentUICulture = Properties.Resources.Culture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Program.isLoaded())
            {
                MessageBox.Show(Resources.AlreadyRunning, Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }


            mainForm = new MainForm();
            mainForm.Icon = Properties.Resources.app48;
            Application.Run();

        }


        #region isLoaded()
        /// <summary>
        /// Does it has been already running
        /// </summary>
        /// <returns>true = Running, false = Not running</returns>
        private static bool isLoaded()
        {
            // Create mutex
            _mutex = new System.Threading.Mutex(false, Application.ProductName);
            // Check mutex
            if (_mutex.WaitOne(0, false) == false)
            {
                // Already running
                return true;
            }
            return false;
        }
        #endregion
    }
}
