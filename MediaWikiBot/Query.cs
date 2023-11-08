using System.Text.Json.Serialization;

namespace MediaWikiBot;
public class Query
{
    [JsonPropertyName("pages")]
    public Dictionary<string, Page>? Pages { get; set; }

    [JsonPropertyName("querypage")]
    public QueryPage? QueryPage { get; set; }
}
