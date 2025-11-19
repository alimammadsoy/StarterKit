namespace StarterKit.Application.Exceptions
{
    public class UserAlreadyExistedException : Exception
    {
        public UserAlreadyExistedException()
        {
        }

        public UserAlreadyExistedException(string? message) : base(message)
        {
        }

        public UserAlreadyExistedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
