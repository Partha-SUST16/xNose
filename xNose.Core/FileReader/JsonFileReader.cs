using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace xNose.Core.FileReader
{
    public class Result
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
    public static class JsonFileReader
    {
        public static List<Result> ReadResultFile(string jsonFilePath) 
        {
            var jsonContent = File.ReadAllText(jsonFilePath);
            return JsonSerializer.Deserialize<List<Result>>(jsonContent);
        }
    }
}
