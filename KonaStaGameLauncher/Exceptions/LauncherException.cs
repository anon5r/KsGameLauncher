using System;
using System.Runtime.Serialization;

namespace KsGameLauncher
{
    [Serializable]
    internal class LauncherException : Exception
    {
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
    }
}