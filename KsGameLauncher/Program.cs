using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
#if DEBUG
using System.Diagnostics;
#endif
namespace KsGameLauncher
{
    internal static class Program
    {

        private static System.Threading.Mutex _mutex;


        /// <summary>
        /// Main entrypoint of an application
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Setup user language
            try
            {
#if DEBUG
                Debug.WriteLine(String.Format("User culture: {0}", CultureInfo.CurrentUICulture.Name));
#endif
                Properties.Resources.Culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
            }
            catch
            {
                Properties.Resources.Culture = new CultureInfo("en-US");
            }
            //Properties.Resources.Culture = new CultureInfo("ja-JP");


            // Default displayed default messages on C#
            Thread.CurrentThread.CurrentCulture = Properties.Resources.Culture;
            Thread.CurrentThread.CurrentUICulture = Properties.Resources.Culture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Program.IsLoaded())
            {
                MessageBox.Show(Resources.AlreadyRunning, Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }


            new MainForm
            {
                Icon = Properties.Resources.app48
            };
            Application.Run();

        }


        #region isLoaded()
        /// <summary>
        /// Does it has been already running
        /// </summary>
        /// <returns>true = Running, false = Not running</returns>
        private static bool IsLoaded()
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
