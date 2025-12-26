namespace StarterKit.Application.Exceptions
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; }
        public object ErrorObject { get; }

        public CustomHttpException(int statusCode, object errorObject)
        {
            StatusCode = statusCode;
            ErrorObject = errorObject;
        }
    }
}