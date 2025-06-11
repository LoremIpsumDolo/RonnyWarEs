using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CsvToJson;

public static class Utils
{
    // Anpassen !
    private const string FileRoot = @"";

    public static IEnumerable<string> ReadFile(string filename)
    {
        var path = Path.Combine(FileRoot, filename);
        try
        {
            return File.ReadAllLines(path)
                .Select(zeile => zeile.Trim())
                .Where(zeile => !string.IsNullOrWhiteSpace(zeile));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen der Datei: {ex.Message}");
            return [];
        }
    }

    public static void WriteJson(string data, string filename)
    {
        var path = Path.Combine(FileRoot, filename);
        File.WriteAllText(path, data);
    }

    public static string ConvertDateStringToString(string rawDate)
    {
        return DateTime.TryParseExact(rawDate, "yyyyMMdd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out DateTime date)
            ? date.ToString("yyyy-MM-dd")
            : "";
    }

    public static DateTime? ConvertDateStringToDateTime(string input)
    {
        if (DateTime.TryParseExact(input, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date))
        {
            return date;
        }

        return null;
    }
}

public class DateOnlyJsonConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
