using System.Text.Json.Serialization;

namespace MediaWikiBot;

public class Result
{
    [JsonPropertyName("query")]
    public Query? Query { get; set; }

    [JsonPropertyName("error")]
    public Dictionary<string, string>? Error { get; set; }
}
