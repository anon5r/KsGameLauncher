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

        internal static string[] args;

        private static System.Threading.Mutex _mutex;

        internal static MainContext mainContext;

        /// <summary>
        /// Main entrypoint of an application
        /// </summary>
        [STAThread]
        //static void Main(string[] args)
        static void Main(string[] args)
        {
            Program.args = new string[args.Length];
            Program.args = args;


            if (args.Length == 1)
            {

                Thread t = new Thread(new ThreadStart(async () =>
                {
                    // Set as running on background
                    MainContext.RunBackground = true;

                    try
                    {
                        if (Uri.TryCreate(args[0], UriKind.Absolute, out Uri uri))
                        {
                            await ProcessUri(uri);
                            // Exit app when the game launched
                            Application.Exit();
                        }
                    }
                    catch (FormatException ex)
                    {
                        MessageBox.Show("Unknown parameters specified. " + ex.Message, Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (mainContext != null)
                    {
                        mainContext.ExitingProcess();
                    }
                    return;

                }));
                t.SetApartmentState(System.Threading.ApartmentState.STA);
                t.Start();

            }


            // Normal startup



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


            // Migrate user settings before version
            if (!Properties.Settings.Default.IsUpgrated)
            {
                Properties.Settings.Default.Upgrade();
                // Mark as Upgraded
                Properties.Settings.Default.IsUpgrated = true;
                Properties.Settings.Default.Save();
            }



            if (!MainContext.RunBackground && Program.IsLoaded())
            {
                MessageBox.Show(Resources.AlreadyRunning, Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            mainContext = new MainContext(new MainForm
            {
                Text = Resources.AppName,
                Icon = Properties.Resources.appIcon,
                // Hide from taskbar
                ShowInTaskbar = false,
                WindowState = FormWindowState.Minimized,
            });
            Application.Run(mainContext);
        }

        /// <summary>
        /// Processing custom URI for <see cref="Properties.Settings.Default.AppUriScheme"/>
        /// </summary>
        /// <param name="uri">Custom URI starts with `Properties.Settings.Default.AppUriScheme`</param>
        static async Task ProcessUri(Uri uri)
        {
            if (string.Equals(uri.Scheme, Properties.Settings.Default.AppUriScheme, StringComparison.OrdinalIgnoreCase))
            {

#if DEBUG
                Debug.WriteLine(string.Format("Scheme: {0}", uri.Scheme));
                Debug.WriteLine(string.Format("Host: {0}", uri.Host));
                Debug.WriteLine(string.Format("Port: {0}", uri.Port.ToString()));
                Debug.WriteLine(string.Format("LocalPath: {0}", uri.LocalPath));
                Debug.WriteLine(string.Format("AbsolutePath: {0}", uri.AbsolutePath));
                Debug.WriteLine(string.Format("Query: {0}", uri.Query));
#endif
                // With Custom URI
                switch (uri.Host)
                {
                    // Launch action
                    case "launch":
                        await Launcher.LaunchGames(uri.LocalPath.Substring(1));
                        break;

                    default:
                        MessageBox.Show(string.Format("Unknown action \"{0}\" specified.", uri.Host), Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Incorrect parameters specified.", Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
