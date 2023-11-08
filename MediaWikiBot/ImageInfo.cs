using System.Text.Json.Serialization;

namespace MediaWikiBot
{
    public class ImageInfo
    {
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
        [JsonPropertyName("userid")]
        public int? UserId { get; set; }
        [JsonPropertyName("size")]
        public int? Size { get; set; }
        [JsonPropertyName("width")]
        public int? Width { get; set; }
        [JsonPropertyName("height")]
        public int? Height { get; set; }
        [JsonPropertyName("parsedcomment")]
        public string? ParsedComment { get; set; }
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
        [JsonPropertyName("html")]
        public string? Html { get; set; }
        [JsonPropertyName("canonicaltitle")]
        public string? CanonicalTitle { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("descriptionurl")]
        public string? DescriptionUrl { get; set; }
        [JsonPropertyName("descriptionshorturl")]
        public string? DescriptionShortUrl { get; set; }
        [JsonPropertyName("sha1")]
        public string? Sha1 { get; set; }
        [JsonPropertyName("metadata")]
        public NameValue[]? Metadata { get; set; }
        [JsonPropertyName("commonmetadata")]
        public NameValue[]? CommonMetadata { get; set; }
        [JsonPropertyName("extmetadata")]
        public Dictionary<string, ExtMetadata>? ExtMetadata { get; set; }
        [JsonPropertyName("mime")]
        public string? Mime { get; set; }
        [JsonPropertyName("mediatype")]
        public string? MediaType { get; set; }
        [JsonPropertyName("bitdepth")]
        public int BitDepth { get; set; }

        [JsonIgnore]
        public byte[]? ImageContent { get; set; }
    }
}