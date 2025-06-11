using System.Globalization;
using System.Text.Json;

namespace CsvToJson;

public static class Program
{
    public static void Main()
    {
        const string inputFilename = "KL_Tageswerte_Beschreibung_Stationen.txt";
        const string outputFilename = "KL_Tageswerte_Beschreibung_Stationen.json";
        // in Utils: FileRoot anpassen!  

        var rawFileContent = Utils.ReadFile(inputFilename);
        var rowItems = CsvParser.GetRowItems(rawFileContent);
        var stationsData = StationFactory.MakeStations(rowItems);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        options.Converters.Add(new DateOnlyJsonConverter());
        var jsonData = JsonSerializer.Serialize(stationsData, options);
        Utils.WriteJson(jsonData, outputFilename);
    }
}

public static class StationFactory
{
    public static StationDataExport MakeStations(List<List<string>> rowItems)
    {
        Console.WriteLine("making Stations");

        var stations = new Dictionary<string?, Station>();

        foreach (var rowItem in rowItems.Skip(2))
        {
            var station = new Station
            {
                StationId = rowItem[0],
                FromDate = Utils.ConvertDateStringToDateTime(rowItem[1]),
                ToDate = Utils.ConvertDateStringToDateTime(rowItem[2]),
                Longitude = float.Parse(rowItem[3]),
                Latitude = float.Parse(rowItem[4])
            };
            stations[station.StationId] = station;
        }

        return new StationDataExport
        {
            Data = stations,
            LastChecked = DateTime.Today.ToString(CultureInfo.CurrentCulture)
        };
    }
}

public class StationDataExport
{
    public Dictionary<string?, Station> Data { get; set; } = new();
    public string LastChecked { get; set; } = "";
}

public class Station
{
    public string? StationId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public float Longitude { get; init; }
    public float Latitude { get; init; }
}

public static class CsvParser
{
    private const string SplitOption = " ";

    public static List<List<string>> GetRowItems(IEnumerable<string> rawFileContent)
    {
        return rawFileContent.Select(SplitRow).ToList();
    }

    private static List<string> SplitRow(string row)
    {
        return row.Split(SplitOption, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}