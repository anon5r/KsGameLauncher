using System.Runtime.Serialization;

namespace KsGameLauncher2
{
    [Serializable]
    internal class LoginCancelException : Exception
    {
        public LoginCancelException()
        {
        }

        public LoginCancelException(string message) : base(message)
        {
        }

        public LoginCancelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LoginCancelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}