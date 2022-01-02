using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace KsGameLauncher
{

    /// <summary>
    /// Game apps information structure
    /// </summary>
    class AppInfo
    {
        [JsonPropertyName("game_id")]
        public string ID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("launch")]
        public AppInfoLaunch Launch { get; set; }

        [JsonPropertyName("iconFile")]
        public string IconPath { get; set; }
        [JsonIgnore]
        private System.Drawing.Icon _icon;

        public System.Drawing.Icon GetIcon()
        {
            if (_icon == null)
            {
                try
                {
                    string path = IconPath;
                    if (path.StartsWith("{{installDir}}"))
                    {
                        // Load installDir path from Registory HKEY_LOCAL_MACHINE\SOFTWARE\KONAMI\{name}\installDir
                        string installDir = Utils.GameRegistry.GetInstallDir(this.Name);
                        if (installDir == null)
                            throw new Exception(String.Format("Game \"{0}\" is not installed", this.Name));
                        path = IconPath.Replace("{{installDir}}", installDir);
                    }

                    _icon = new System.Drawing.Icon(path);
                }
                catch
                {
                    _icon = Properties.Resources.default_game_icon;
                }
            }
            return _icon;
        }

        internal class AppInfoLaunch
        {
            [JsonPropertyName("url")]
            public string URL { get; set; }
            [JsonPropertyName("selector")]
            public string Selector { get; set; }
            public Uri GetUri()
            {
                return new Uri(URL);
            }
        }






        [JsonIgnore]
        private static List<AppInfo> List = new List<AppInfo>();

        public static List<AppInfo> GetList()
        {
            return List;
        }

        public static bool ContainID(string id)
        {
            bool found = false;
            List.ForEach((AppInfo app) =>
            {
                if (app.ID.Equals(id))
                    found = true;
            });
            return found;
        }

        internal static List<AppInfo> LoadFromJson(string json)
        {
            try
            {
                List = JsonSerializer.Deserialize<List<AppInfo>>(json);
            }
            catch (Exception)
            {
                return null;
            }

            return List;
        }

        /// <summary>
        /// Find appinfo
        /// </summary>
        /// <param name="id"></param>
        /// <returns>If find it, will return AppInfo, otherwise returns null</returns>
        public static AppInfo Find(string id)
        {
            foreach (AppInfo app in List)
            {
                if (app.ID.Equals(id))
                    return app;
            }
            return null;
        }

        async internal static void Save()
        {
            try
            {
                using (FileStream fstream = new FileStream(Properties.Settings.Default.appInfoLocal, FileMode.Create,
                           FileAccess.Write))
                {
                    if (!fstream.CanWrite)
                    {
                        MessageBox.Show(Resources.CannotSaveGameList);
                        return;
                    }

                    string json = JsonSerializer.Serialize(List);
                    byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
                    await fstream.WriteAsync(jsonBytes, 0, json.Length);
                    fstream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

    }

}
