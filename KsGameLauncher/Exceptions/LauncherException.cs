﻿using System;
using System.Runtime.Serialization;

namespace KsGameLauncher
{
    [Serializable]
    internal class LauncherException : Exception
    {

        public Uri OpenURL { get; }

        public LauncherException()
        {
        }

        public LauncherException(string message) : base(message)
        {
        }

        public LauncherException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LauncherException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


        public LauncherException(string message, string openURL) : base(message)
        {
            this.OpenURL = new Uri(openURL);
        }

        public LauncherException(string message, Uri openURI) : base(message)
        {
            this.OpenURL = openURI;
        }
    }
}