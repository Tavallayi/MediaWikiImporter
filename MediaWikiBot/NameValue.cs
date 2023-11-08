using System.Text.Json.Serialization;

namespace MediaWikiBot;

public class NameValue
{
    [JsonPropertyName("value")]
    public object? Value { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}