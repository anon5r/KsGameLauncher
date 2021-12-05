using System;
using System.Text.Json.Serialization;

namespace KonaStaGameLauncher
{

    /// <summary>
    /// Game apps information structure
    /// </summary>
    class AppInfo
    {
        [JsonPropertyName("id")]
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
                    // Load installDir path from Registory HKEY_LOCAL_MACHINE\SOFTWARE\KONAMI\{name}\installDir
                    string installDir = Utils.GameRegistry.GetInstallDir(this.Name);
                    if (installDir == null)
                        throw new Exception(String.Format("Game \"{0}\" is not installed", this.Name));
                    _icon = new System.Drawing.Icon(IconPath.Replace("{{installDir}}", installDir));
                }
                catch
                {
                    _icon = Properties.Resources.ealogo;
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

    }

}
