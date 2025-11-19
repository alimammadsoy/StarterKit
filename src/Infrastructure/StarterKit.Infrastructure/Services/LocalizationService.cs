using Microsoft.AspNetCore.Hosting;
using StarterKit.Application.Abstractions.Services;
using System.Text.Json;

namespace StarterKit.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _resources;

        public LocalizationService(IWebHostEnvironment env)
        {
            _resources = new();

            var folder = Path.Combine(env.ContentRootPath, "Localization");

            Load("az", Path.Combine(folder, "az.json"));
            Load("en", Path.Combine(folder, "en.json"));
            Load("ru", Path.Combine(folder, "ru.json"));
        }

        private void Load(string lang, string path)
        {
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            if (dict != null)
                _resources[lang] = dict;
        }

        public string GetMessage(string key, string lang)
        {
            lang = lang.ToLower();

            if (!_resources.ContainsKey(lang))
                lang = "az"; // default

            if (_resources[lang].TryGetValue(key, out string? value))
                return value;

            return key; // key tapılmazsa özünü qaytarır
        }
    }

}
