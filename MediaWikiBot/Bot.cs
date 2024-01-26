using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MediaWikiBot;

public class Bot
{
    public Bot(IConfiguration configuration, string wikiKey, ILogger logger)
    {
        Configuration = configuration;
        url = Configuration[$"{wikiKey}:url"] ?? throw new InvalidDataException("Invalid url!");
        username = Configuration[$"{wikiKey}:username"];
        password = Configuration[$"{wikiKey}:password"];
        WikiKey = wikiKey;
        Logger = logger;
        innerHttpClient = new HttpClient();
        //CheckApiUrl();
    }

    private void CheckApiUrl()
    {
        Logger.LogInformation($"CheckApiUrl: {apiUrl}");
        var start = DateTime.Now;
        innerHttpClient.Timeout = TimeSpan.FromSeconds(10);
        var resp = innerHttpClient.GetAsync(apiUrl, new HttpCompletionOption()).Result;
        if (resp == null || resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new InvalidOperationException("Invalid api.");
        }
        Logger.LogInformation($"CheckApiUrl: {apiUrl} done({(DateTime.Now - start).TotalMilliseconds}).");
    }

    public IConfiguration Configuration { get; }
    public string WikiKey { get; }
    public ILogger Logger { get; }

    private HttpClient innerHttpClient;

    public string? LoginToken { get; private set; }
    public string? CsrfToken { get; private set; }

    private readonly string url;
    private readonly string? username;
    private readonly string? password;

    private string apiUrl => $"{url}/api.php";

    public async Task Login()
    {
        Logger.Log(LogLevel.Information, "Login");
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }
        if (!string.IsNullOrEmpty(CsrfToken))
        {
            return;
        }
        await TakeLoginToken();
        Logger.Log(LogLevel.Information, "TakeLoginToken");
        await TakeCsrfToken();
        Logger.Log(LogLevel.Information, "TakeCsrfToken");

    }

    private async Task TakeCsrfToken()
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }
        FormUrlEncodedContent obj = new FormUrlEncodedContent(
            new Dictionary<string, string>()
        {
            { "action", "login" },
            { "lgname", username },
            { "lgpassword", password },
            { "format", "json" },
            { "lgtoken", LoginToken! }
        }); ;


        var response = await innerHttpClient.PostAsync($"{apiUrl}", obj);
        if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new InvalidOperationException("Login failed.");
        }
        var content = await response.Content.ReadAsStringAsync();


        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("action", "query");
        queryString.Add("meta", "tokens");
        queryString.Add("format", "json");

        var login = await innerHttpClient.GetFromJsonAsync<JsonObject>($"{apiUrl}?{queryString}");
        if (login == null)
        {
            throw new InvalidOperationException("Login failed.");
        }
        CsrfToken = login?["query"]?["tokens"]?["csrftoken"]?.GetValue<string>();
    }

    private async Task TakeLoginToken()
    {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("action", "query");
        queryString.Add("meta", "tokens");
        queryString.Add("type", "login");
        queryString.Add("format", "json");

        try
        {
            var login = await innerHttpClient.GetFromJsonAsync<JsonObject>($"{apiUrl}?{queryString}");
            if (login == null)
            {
                throw new InvalidOperationException("Login failed.");
            }
            LoginToken = login?["query"]?["tokens"]?["logintoken"]?.GetValue<string>();

        }
        catch
        {

            throw;
        }
    }

    public async Task Save(string pageTitle, string pageContent)
    {
        await Login();

        FormUrlEncodedContent obj = new FormUrlEncodedContent(
            new Dictionary<string, string>()
            {
                {"action",  "edit"},
                {"title",  pageTitle},
                {"token",  CsrfToken!},
                {"format",  "json"},
                {"text",  pageContent},
            }); ;
        var start = DateTime.Now;
        Logger.Log(LogLevel.Information, $"Saving {pageTitle}");
        var response = await innerHttpClient.PostAsync($"{apiUrl}", obj);
        Logger.Log(LogLevel.Information, $"{pageTitle} Saved({(DateTime.Now - start).TotalMilliseconds})");
        if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new InvalidOperationException("Saveing page failed.");
        }
    }

    public async Task<Page[]?> GetPage(params string[] pageTitles)
    {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("action", "query");
        queryString.Add("format", "json");
        queryString.Add("prop", "revisions");
        queryString.Add("titles", string.Join("|", pageTitles));
        queryString.Add("rvprop", "content");

        var start = DateTime.Now;
        Logger.LogInformation($"Url: {apiUrl}?{queryString}");
        var querystr = await innerHttpClient.GetStringAsync($"{apiUrl}?{queryString}");
        Logger.LogInformation($"Url done: {apiUrl}?{queryString}({(DateTime.Now - start).TotalMilliseconds})");

        var result = JsonSerializer.Deserialize<Result>(querystr);
        if (result == null)
        {
            throw new InvalidOperationException("Page not found.");
        }

        if (result.Error != null)
        {
            throw new InvalidOperationException(JsonSerializer.Serialize(result.Error));
        }

        return result?.Query?.Pages?.Values?.ToArray();
    }

    public async Task<Page[]?> GetFileInfo(bool downloadImages, params string[] files)
    {
        Logger.LogInformation($"Getting file info: {string.Join(",", files)}");
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("action", "query");
        queryString.Add("format", "json");
        queryString.Add("prop", "imageinfo");
        queryString.Add("titles", string.Join("|", files));
        queryString.Add("iiprop", "url|archivename|badfile|bitdepth|canonicaltitle|comment|commonmetadata|dimensions|extmetadata|mediatype|metadata|mime|parsedcomment|sha1|size|thumbmime|timestamp|uploadwarning|user|userid");

        var start = DateTime.Now;
        Logger.LogInformation($"Url: {apiUrl}?{queryString}");
        var querystr = await innerHttpClient.GetStringAsync($"{apiUrl}?{queryString}");
        var result = JsonSerializer.Deserialize<Result>(querystr);
        if (result == null)
        {
            throw new InvalidOperationException("Page not found.");
        }
        Logger.LogInformation($"Getting file info done: {string.Join(",", files)}({(DateTime.Now - start).TotalMilliseconds})");
        start = DateTime.Now;
        var filesData = result?.Query?.Pages?.Values?.ToArray();
        if (downloadImages && filesData != null)
        {
            foreach (var item in filesData)
            {
                foreach (var ii in item.ImageInfo ?? Array.Empty<ImageInfo>())
                {
                    innerHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                    Logger.LogInformation($"File is downloading: {ii.Url}({(DateTime.Now - start).TotalMilliseconds})");
                    ii.ImageContent = await innerHttpClient.GetByteArrayAsync(ii.Url!);
                    Logger.LogInformation($"File is downloaded: {ii.Url}({(DateTime.Now - start).TotalMilliseconds})");
                }
            }
        }
        return filesData;
    }

    public async Task UploadImage(string title, byte[]? imageContent, string mimetype)
    {
        if (imageContent is null) return;
        await Login();
        Logger.LogInformation($"{title}: uploading.");

        var content = new MultipartFormDataContent();
        content.Add(new StringContent("upload"), "action");
        content.Add(new StringContent(title), "filename");
        content.Add(new StringContent("1"), "ignorewarnings");
        content.Add(new StringContent(CsrfToken!), "token");
        content.Add(new StringContent("json"), "format");
        using MemoryStream ms = new MemoryStream(imageContent);
        content.Add(new StreamContent(ms), "file", title.Replace("\"", string.Empty));

        var response = await innerHttpClient.PostAsync($"{apiUrl}", content);
        var str = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonObject>(str);
        Logger.LogInformation($"{title}: upload {json?["upload"]?["result"]}");
    }

    public async Task<Query?> WantedPages(int limit)
    {
        return await QueryPage("Wantedpages", limit);
    }

    public async Task<Query?> WantedFiles(int limit)
    {
        return await QueryPage("Wantedfiles", limit);
    }

    public async Task<Query?> QueryPage(string pageType, int limit)
    {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("action", "query");
        queryString.Add("format", "json");
        queryString.Add("list", "querypage");
        queryString.Add("formatversion", "2");
        queryString.Add("qppage", pageType);
        queryString.Add("qplimit", limit.ToString());
        Logger.Log(LogLevel.Information, $"{apiUrl}?{queryString}");
        //var str = await innerHttpClient.GetStringAsync($"{apiUrl}?{queryString}");
        var qp = await innerHttpClient.GetFromJsonAsync<Result>($"{apiUrl}?{queryString}");
        if (qp == null)
        {
            throw new InvalidOperationException("Invalid query page.");
        }
        return qp.Query;
    }

    public async Task<Query?> Search(string search)
    {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("format", "json");
        queryString.Add("action", "query");
        queryString.Add("list", "search");
        queryString.Add("formatversion", "2");
        queryString.Add("srsearch", search);

        var str = await innerHttpClient.GetStringAsync($"{apiUrl}?{queryString}");
        var qp = await innerHttpClient.GetFromJsonAsync<Result>($"{apiUrl}?{queryString}");
        if (qp == null)
        {
            throw new InvalidOperationException("Invalid query page.");
        }
        return qp.Query;
    }
}