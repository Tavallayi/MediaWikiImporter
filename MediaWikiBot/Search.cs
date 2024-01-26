using System.Text.Json.Serialization;

namespace MediaWikiBot;

public class Search
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("pageid")]
    public int Pageid { get; set; }
    [JsonPropertyName("size")]
    public int Size { get; set; }
    [JsonPropertyName("wordcount")]
    public int Wordcount { get; set; }
    [JsonPropertyName("snippet")]
    public string? Snippet { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}