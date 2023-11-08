using System.Text.Json.Serialization;

namespace MediaWikiBot;

public class Revision
{
    [JsonPropertyName("revid")]
    public int RevId { get; set; }
    [JsonPropertyName("parentid")]
    public int ParentId { get; set; }
    [JsonPropertyName("minor")]
    public bool Minor { get; set; }
    [JsonPropertyName("user")]
    public string? User { get; set; }
    [JsonPropertyName("userid")]
    public int UserId { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }
    [JsonPropertyName("size")]
    public int Size { get; set; }
    [JsonPropertyName("sha1")]
    public string? Sha1 { get; set; }
    [JsonPropertyName("roles")]
    public string[]? Roles { get; set; }
    [JsonPropertyName("contentmodel")]
    public string? ContentModel { get; set; }
    [JsonPropertyName("contentformat")]
    public string? ContentFormat { get; set; }
    [JsonPropertyName("*")]
    public string? Content { get; set; }
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
    [JsonPropertyName("parsedcomment")]
    public string? ParsedComment { get; set; }
    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
}