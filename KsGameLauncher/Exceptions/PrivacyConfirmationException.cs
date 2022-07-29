using System;
using System.Runtime.Serialization;

namespace KsGameLauncher
{
    [Serializable]
    internal class PrivacyConfirmationException : Exception
    {
        public static string PrivacyURL;

        public PrivacyConfirmationException()
        {
        }

        public PrivacyConfirmationException(string message) : base(message)
        {
        }

        public PrivacyConfirmationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PrivacyConfirmationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PrivacyConfirmationException(string message, string URL) : base(message)
        {
            PrivacyURL = URL;
        }

        public PrivacyConfirmationException(string message, string URL, Exception innerException) : base(message, innerException)
        {
            PrivacyURL = URL;
        }

        public void SetTosURL(string url)
        {
            PrivacyURL = url ?? throw new ArgumentNullException("url");
        }
        public void SetTosURL(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            PrivacyURL = url.ToString();
        }
        public string GetPrivacyURL()
        {
            return PrivacyURL;
        }
    }
}