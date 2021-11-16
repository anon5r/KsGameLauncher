using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
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
        [JsonPropertyName("login")]
        public AppInfoLogin Login { get; set; }
        [JsonPropertyName("iconFile")]
        public string IconPath { get; set; }
        [JsonIgnore]
        private System.Drawing.Icon _icon;
        [JsonIgnore]
        private bool IsInstalled = false;

        public System.Drawing.Icon GetIcon()
        {
            if (_icon == null)
            {
                try
                {
                    // Load installDir path from Registory HKEY_LOCAL_MACHINE\SOFTWARE\KONAMI\{name}\installDir
                    string installDir = Utils.GameRegistry.GetInstallDir(this.Name);
                    if (installDir == null)
                        throw new Exception("Game \"" + this.Name + "\" is not installed");
                    _icon = new System.Drawing.Icon(IconPath.Replace("{{installDir}}", installDir));
                }
                catch
                {
                    _icon = Properties.Resources.ealogo;
                }
            }
            return _icon;
        }

        internal class AppInfoLogin
        {
            [JsonPropertyName("url")]
            public string URL { get; set; }
            [JsonPropertyName("xpath")]
            public string Xpath { get; set; }
            public Uri GetUri()
            {
                return new Uri(URL);
            }
        }

    }

}
