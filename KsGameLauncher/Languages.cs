using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;

namespace KsGameLauncher
{

    internal class Languages
    {
        private static Dictionary<string, Language> _list = new Dictionary<string, Language>();
        internal static System.Collections.Specialized.StringCollection SupportedLanguages = Properties.Settings.Default.SupportedLanguage;

        public static Language DefaultLanguage { 
            get { return new Language(Properties.Resources.DefaultLanguage); } 
        }

        private static void _Init()
        {
            if (_list == null || _list.Count < 1)
            {
                // Add system language
                Language systemLang = new Language(Properties.Resources.DefaultLanguage);
                _list.Add(Properties.Resources.DefaultLanguage, systemLang);

                // Lists all languages
                foreach (CultureInfo culinfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
                {
#if DEBUG
                    //Debug.WriteLine(String.Format("Culture: {0}", culinfo.Name));
#endif
                    if (SupportedLanguages.Contains(culinfo.Name))
                    {
#if DEBUG
                        Debug.WriteLine(String.Format("Supported culture: {0}", culinfo.Name));
#endif
                        _list.Add(culinfo.Name, new Language(culinfo.Name));
                    }
                }
            }
        }

        public static List<Language> GetAvailableLanguages()
        {
            _Init();
            // Convert List from Dictionary
            List<Language> list = new List<Language>();
            foreach (string key in _list.Keys)
                list.Add(_list[key]);
            return list;
        }

        public static Language GetLanguage(string lang)
        {
            _Init();
            if (_list.ContainsKey(lang))
                return _list[lang];
            return _list[Properties.Resources.DefaultLanguage];
        }

    }
    
    internal class Language
    {
        internal CultureInfo _Culture;
        internal string _name;
        internal string _displayName;
        internal string _idString;
        public CultureInfo Culture { get { return _Culture; } }
        public string Name { get { return _name; } }
        public string DisplayName { get { return _displayName; } }
        public string ID { get { return _idString; } }
        public override string ToString() { return DisplayName; }


        public Language(string? id)
        {
            if (!string.IsNullOrEmpty(id) && id != Properties.Resources.DefaultLanguage)
            {
                _Culture = new CultureInfo(id!);
                _displayName = _Culture.NativeName;
                _idString = _Culture.Name;
            }
            else
            {
                _Culture = CultureInfo.InstalledUICulture;
                _displayName = Properties.Strings.SystemText;
                _idString = Properties.Resources.DefaultLanguage;
            }
            _name = Languages.SupportedLanguages.Contains(_Culture.Name)
                ? _Culture.Name
                : Properties.Resources.DefaultInternalLanguage;
        }
    }
}
