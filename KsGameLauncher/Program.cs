using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
#if DEBUG
using System.Diagnostics;
#endif
namespace KsGameLauncher
{
    internal static class Program
    {

        private static System.Threading.Mutex _mutex;

        static internal MainForm mainForm;


        /// <summary>
        /// Main entrypoint of an application
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                if (Uri.TryCreate(args[0], UriKind.Absolute, out var uri) &&
                    string.Equals(uri.Scheme, Properties.Settings.Default.AppUriScheme, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine(string.Format("Scheme: {0}", uri.Scheme));
                    Debug.WriteLine(string.Format("Host: {0}", uri.Host));
                    Debug.WriteLine(string.Format("Port: {0}", uri.Port.ToString()));
                    Debug.WriteLine(string.Format("LocalPath: {0}", uri.LocalPath));
                    Debug.WriteLine(string.Format("AbsolutePath: {0}", uri.AbsolutePath));
                    Debug.WriteLine(string.Format("Query: {0}", uri.Query));
                    // With Custom URI
                    switch (uri.Host)
                    {
                        // Launch action
                        case "launch":
                            LaunchGames(uri.LocalPath.Substring(1));
                            break;

                        default:
                            MessageBox.Show("Unknown action specified.", Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    Application.Exit();
                    return;
                }
                else
                {
                    MessageBox.Show("Incorrect parameters specified.", Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            /// Normal startup



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
                return;
            }


            mainForm = new MainForm
            {
                Icon = Properties.Resources.app
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


        private async static void LaunchGames(string gameID)
        {
            string json = await Launcher.GetJson();
            if (json == null)
            {
                MessageBox.Show("There are no games", Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppInfo.LoadFromJson(json);

            if (!AppInfo.ContainID(gameID))
            {
                MessageBox.Show("Unsupported game specified", Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppInfo appInfo = AppInfo.Find(gameID);

            if (appInfo == null)
            {
                MessageBox.Show("Cannot find that game you specified", Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Launcher launcher = Launcher.Create();
                launcher.StartApp(appInfo);
            }
            catch (LoginException ex)
            {
                MessageBox.Show(String.Format(
                    "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                    appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                , Resources.ErrorWhileLogin, MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            catch (LauncherException ex)
            {
                MessageBox.Show(String.Format(
                    "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                    appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(
                    "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                    appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
