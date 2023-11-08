using System.Text.Json.Serialization;

namespace MediaWikiBot
{
    public class QueryPage
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("results")]
        public QueryPageResult[]? Results { get; set; }
    }
}