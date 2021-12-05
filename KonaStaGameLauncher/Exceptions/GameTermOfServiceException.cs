using System;
using System.Runtime.Serialization;

namespace KonaStaGameLauncher
{
    [Serializable]
    internal class GameTermOfServiceException : Exception
    {
        public GameTermOfServiceException()
        {
        }

        public GameTermOfServiceException(string message) : base(message)
        {
        }

        public GameTermOfServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameTermOfServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}