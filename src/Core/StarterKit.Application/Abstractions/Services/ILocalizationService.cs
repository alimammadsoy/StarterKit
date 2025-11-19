namespace StarterKit.Application.Abstractions.Services
{
    public interface ILocalizationService
    {
        string GetMessage(string key, string lang);
    }
}
