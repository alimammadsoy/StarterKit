using FluentValidation.Results;
using System.Text.Json;

namespace StarterKit.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base()
        {
            Errors = new Dictionary<string, string[]>();
        }
        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(
                    failure => JsonNamingPolicy.SnakeCaseLower.ConvertName(failure.PropertyName),
                    failure => failure.ErrorMessage
                )
                .ToDictionary(
                    group => group.Key,
                    group => group.ToArray()
                );
        }
        public IDictionary<string, string[]> Errors { get; }
    }
}
