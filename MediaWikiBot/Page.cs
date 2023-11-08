using System.Text.Json.Serialization;

namespace MediaWikiBot;

public class Page
{
    [JsonPropertyName("pageid")]
    public int PageId { get; set; }
    [JsonPropertyName("ns")]
    public int NS { get; set; }
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("revisions")]
    public Revision[]? Revisions { get; set; }
    [JsonPropertyName("missing")]
    public string? Missing { get; set; }
    [JsonPropertyName("known")]
    public string? Known { get; set; }
    [JsonPropertyName("imagerepository")]
    public string? ImageRepository { get; set; }
    [JsonPropertyName("badfile")]
    public bool BadFile { get; set; }
    [JsonPropertyName("imageinfo")]
    public ImageInfo[]? ImageInfo { get; set; }
}