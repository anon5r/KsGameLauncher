using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
#if DEBUG
using System.Diagnostics;
#endif

namespace KsGameLauncher.Utils
{
    internal class AppUtil
    {
        public static async Task<bool> DownloadJson()
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

            FileInfo finfo = new FileInfo(Properties.Settings.Default.appInfoLocal);
            return (finfo.Length > 0);
        }

    }
}
