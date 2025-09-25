using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StarterKit.WebApi.Configurations
{
    public class AzerbaijanDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly TimeZoneInfo AzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Azerbaijan Standard Time"
                : "Asia/Baku"
        );

        private static readonly string[] Formats = { "dd.MM.yyyy HH:mm", "dd.MM.yyyy" };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string input = reader.GetString();

            if (string.IsNullOrWhiteSpace(input))
                return default;

            if (DateTime.TryParseExact(input, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var localAzTime))
            {
                return TimeZoneInfo.ConvertTimeToUtc(localAzTime, AzTimeZone);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var azTime = TimeZoneInfo.ConvertTimeFromUtc(value, AzTimeZone);
            writer.WriteStringValue(azTime.ToString("dd.MM.yyyy HH:mm"));
        }
    }

}
