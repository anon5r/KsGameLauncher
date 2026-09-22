using System.Runtime.Serialization;

namespace KsGameLauncher2
{
    [Serializable]
    internal class LoginUriException : Exception
    {
        public LoginUriException()
        {
        }

        public LoginUriException(string message) : base(message)
        {
        }

        public LoginUriException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LoginUriException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}