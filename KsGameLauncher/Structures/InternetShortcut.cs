using System;
using System.IO;

namespace KsGameLauncher.Structures
{
    internal class InternetShortcut
    {
        public Uri URL { get; set; }
        public string IconFile { get; set; }
        public string IDList { get; set; }
        public int IconIndex = 0;

        static readonly string[] SearchParams = { "URL=", "IconIndex=", "IconFile=", "IDList=" };

        public static InternetShortcut Parse(StreamReader content)
        {
            var self = new InternetShortcut();
            while (!content.EndOfStream)
            {
                string line = content.ReadLine();
                SetParams(ref self, line);
            }

            return self;
        }

        public static InternetShortcut Parse(string[] content)
        {
            var self = new InternetShortcut();

            foreach (var line in content)
            {
                SetParams(ref self, line);
            }

            return self;
        }

        public static InternetShortcut Parse(string content)
        {
            string[] rows = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            return Parse(rows);
        }

        internal static bool Validate(string content)
        {
            return
                content.Contains("[InternetShortcut]")
                //&& content.Contains("[{000214A0-0000-0000-C000-000000000046}]")
                && content.Contains("\nURL=");
        }

        internal static bool Validate(string[] content)
        {
            return Validate(string.Concat(content));
        }

        internal static bool Validate(StreamReader stream)
        {
            return Validate(stream.ReadToEnd());
        }

        private static void SetParams(ref InternetShortcut obj, string line)
        {

            foreach (string param in SearchParams)
            {
                if (line.StartsWith(param) && line.Length > param.Length)
                {
                    string value = line.Substring(line.IndexOf(param) + param.Length);

                    if (param.StartsWith("URL"))
                    {
                        if (!value.Contains(Uri.SchemeDelimiter))
                            throw new UriFormatException("Invalid URI format");
                        obj.URL = new Uri(value);
                    }
                    if (param.StartsWith("IconFile"))
                    {
                        obj.IconFile = value;
                    }
                    if (param.StartsWith("IDList"))
                    {
                        obj.IDList = value;
                    }
                    if (param.StartsWith("IconIndex"))
                    {
                        obj.IconIndex = Int32.Parse(value);
                    }
                }
            }
        }
    }
}
