using System.Text.RegularExpressions;

namespace StarterKit.WebApi.Configurations
{
    public class KebabCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            // Convert "ResitExamPlans" → "resit-exam-plans"
            return Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}
