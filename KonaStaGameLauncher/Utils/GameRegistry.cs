using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
#if DEBUG
#endif

namespace KonaStaGameLauncher.Utils
{
    internal class GameRegistry
    {
        private const string REG_BASE_KEY = @"SOFTWARE\KONAMI";


        public static List<string> GetInstalledGameList()
        {
            List<string> list = new List<string>();

            RegistryKey rKey = Registry.LocalMachine.OpenSubKey(REG_BASE_KEY);
            if (rKey.SubKeyCount > 0)
            {
                foreach (string subKey in rKey.GetSubKeyNames())
                {
                    list.Add(subKey);
                }
            }

            return list;
        }


        protected static RegistryKey GetRegistryKey(string GameName)
        {
            return Registry.LocalMachine.OpenSubKey(REG_BASE_KEY + "\\" + GameName);
        }


        /// <summary>
        /// Get registry value from specified key with game name
        /// </summary>
        /// <param name="GameName">Game name</param>
        /// <param name="FindKey">Registy key</param>
        /// <returns>Registry object, if not found key, will return "null"</returns>
        public static object GetValue(string GameName, string FindKey)
        {
            RegistryKey RegKey = GetRegistryKey(GameName);

            if (RegKey != null && RegKey.GetValueNames().Contains<string>(FindKey))
            {
                return RegKey.GetValue(FindKey);
            }
            return null;
        }

        /// <summary>
        /// Find game key
        /// </summary>
        /// <param name="GameName"></param>
        /// <returns>True: Installed, False: Not installed</returns>
        public static bool IsInstalled(string GameName)
        {
            return GetRegistryKey(GameName) != null;
        }


        /// <summary>
        /// Get installed directory path
        /// </summary>
        /// <param name="GameName"></param>
        /// <returns>Installed directory path as string</returns>
        public static string GetInstallDir(string GameName)
        {
            if (string.IsNullOrEmpty(GameName))
                return null;
            if (!IsInstalled(GameName))
                return null;
            return (string)GetValue(GameName, "InstallDir").ToString();
        }

        /// <summary>
        /// Get game resource directory path
        /// </summary>
        /// <param name="GameName"></param>
        /// <returns>Resource directory path as string</returns>
        public static string GetResourceDir(string GameName)
        {
            if (string.IsNullOrEmpty(GameName))
                return null;
            if (!IsInstalled(GameName))
                return null;
            return (string)GetValue(GameName, "ResourceDir").ToString();
        }

        public static string GetLauncherPath(string ProtocolScheme)
        {
            if (string.IsNullOrEmpty(ProtocolScheme))
                return null;

            RegistryKey key = Registry.ClassesRoot.OpenSubKey(ProtocolScheme);
            if (key.SubKeyCount < 1)
                return null;

            string path = key.OpenSubKey(@"shell\open\command").GetValue(null).ToString();
            // path = '"C:\Games\NOSTALGIA\launcher\modules\launcher.exe" "%1"';
            return path.Remove(path.Length - 5).Replace("\"", "").Trim();
        }
    }
}
