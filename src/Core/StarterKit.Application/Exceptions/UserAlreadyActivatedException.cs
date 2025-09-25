namespace StarterKit.Application.Exceptions
{
    public class UserAlreadyActivatedException : Exception
    {
        public UserAlreadyActivatedException()
        {
        }

        public UserAlreadyActivatedException(string? message) : base(message)
        {
        }

        public UserAlreadyActivatedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
