namespace StarterKit.Application.Exceptions
{
    public class ExtensionException : Exception
    {
        public ExtensionException()
        {
        }

        public ExtensionException(string? message) : base(message)
        {
        }

        public ExtensionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
