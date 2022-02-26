using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Reflection;
using System.Windows.Forms;
using AutoUpdaterDotNET;
#if DEBUG
using System.Diagnostics;
#endif

namespace KsGameLauncher.Utils
{
    internal class AppUtil
    {
        public static async Task<bool> DownloadJson()
        {
            string defaultPath = Directory.GetParent(Application.ExecutablePath) + "\\" + Properties.Settings.Default.appInfoLocal;
            return await DownloadJson(defaultPath);
        }

        public static async Task<bool> DownloadJson(string path)
        {

            HttpClient client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(10),
                BaseAddress = new Uri(Properties.Settings.Default.appInfoURL),
            };

#if DEBUG
            Debug.Write(String.Format("There is no {0}", Properties.Settings.Default.appInfoLocal));
            Debug.Write(String.Format("Download and save from {0}", Properties.Settings.Default.appInfoURL));
#endif
            using (Stream jsonStream = await client.GetStreamAsync(Properties.Settings.Default.appInfoURL))
            using (TextReader reader = (new StreamReader(jsonStream)) as TextReader)
            {
                string json = reader.ReadToEnd();
                File.WriteAllText(Properties.Settings.Default.appInfoLocal, json);
            }

            FileInfo finfo = new FileInfo(path);
            return (finfo.Length > 0);
        }

        /// <summary>
        /// Register custom URI scheme
        /// </summary>
        internal static void RegisterScheme()
        {
            try
            {
                //using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(Properties.Settings.Default.AppCustomURIScheme);
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\" + Properties.Settings.Default.AppUriScheme))
                {
                    // Replace typeof(App) by the class that contains the Main method or any class located in the project that produces the exe.
                    // or replace typeof(App).Assembly.Location by anything that gives the full path to the exe
                    string appLocation = Assembly.GetExecutingAssembly().Location;

                    key.SetValue("", "URL:" + Properties.Settings.Default.AppUriScheme);
                    key.SetValue("URL Protocol", "");

                    using (RegistryKey defaultIcon = key.CreateSubKey("DefaultIcon"))
                    {
                        defaultIcon.SetValue("", appLocation + ",1");
                    }

                    using (RegistryKey command = key.CreateSubKey(@"shell\open\command"))
                    {
                        command.SetValue("", "\"" + appLocation + "\" \"%1\"");
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (ArgumentException) { }
        }

        /// <summary>
        /// Unregister custom URI scheme
        /// </summary>
        internal static void DeleteScheme()
        {
            try
            {
                // Remove keys about URI Scheme for this program
                Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\" + Properties.Settings.Default.AppUriScheme);
            }
            catch (ObjectDisposedException) { }
            catch (ArgumentException) { }
        }


        internal static void CheckUpdate()
        {
#if DEBUG
            Debug.WriteLine(String.Format("[CheckUpdate {0}] Start check update", DateTime.Now));
#endif
            AutoUpdater.Synchronous = true;
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Start(Properties.Resources.UpdateXML, typeof(Program).Assembly);
#if DEBUG
            Debug.WriteLine(String.Format("[CheckUpdate {0}] Done called", DateTime.Now));
#endif
        }

    }
}
