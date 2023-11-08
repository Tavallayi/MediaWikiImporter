using System.Text.Json.Serialization;

namespace MediaWikiBot
{
    public class QueryPageResult
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
        [JsonPropertyName("ns")]
        public int Ns { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}