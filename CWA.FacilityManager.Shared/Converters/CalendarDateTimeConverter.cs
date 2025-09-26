using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace CWA.FacilityManager.Shared.Converters
{
    /// <summary>
    /// Custom DateTime converter that preserves date values without timezone conversion issues
    /// </summary>
    public class CalendarDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString))
                return default;

            // Try multiple date formats to ensure compatibility
            var formats = new[]
            {
                DateTimeFormat,
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                "o", // ISO 8601 round-trip format
                "s"  // Sortable date/time format
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    // Return as unspecified kind to prevent automatic timezone conversion
                    return DateTime.SpecifyKind(result, DateTimeKind.Unspecified);
                }
            }

            // Fallback to default parsing
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fallbackResult))
            {
                return DateTime.SpecifyKind(fallbackResult, DateTimeKind.Unspecified);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Write datetime in ISO format without timezone information
            writer.WriteStringValue(value.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
        }
    }
}