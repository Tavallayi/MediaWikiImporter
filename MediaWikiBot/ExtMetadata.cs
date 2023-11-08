using System.Text.Json.Serialization;

namespace MediaWikiBot;

public class ExtMetadata
{
    [JsonPropertyName("value")]
    public object? Value { get; set; }
    [JsonPropertyName("source")]
    public string? Source { get; set; }
    [JsonPropertyName("hidden")]
    public string? Hidden { get; set; }
}