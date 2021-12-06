using System;
using System.Runtime.Serialization;

namespace KonaStaGameLauncher
{
    [Serializable]
    internal class GameTermsOfServiceException : Exception
    {
        public static string TosURL;

        public GameTermsOfServiceException()
        {
        }

        public GameTermsOfServiceException(string message) : base(message)
        {
        }

        public GameTermsOfServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameTermsOfServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GameTermsOfServiceException(string message, string tosURL) : base(message)
        {
            TosURL = tosURL;
        }

        public GameTermsOfServiceException(string message, string tosURL, Exception innerException) : base(message, innerException)
        {
            TosURL = tosURL;
        }

        public void SetTosURL(string url)
        {
            TosURL = url ?? throw new ArgumentNullException("url");
        }
        public void SetTosURL(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            TosURL = url.ToString();
        }
        public string GetTosURL()
        {
            return TosURL;
        }
    }
}