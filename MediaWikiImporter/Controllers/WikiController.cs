using MediaWikiBot;
using Microsoft.AspNetCore.Mvc;

namespace Project1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WikiController : ControllerBase
{
    public WikiController(IConfiguration configuration, ILogger<WikiController> logger)
    {
        Configuration = configuration;
        Logger = logger;
    }

    private IConfiguration Configuration { get; }
    public ILogger Logger { get; }

    [HttpGet]
    [Route("Search")]
    public async Task<IEnumerable<string?>?> SearchAsync(string s)
    {
        Bot bot = new Bot(Configuration, "wikipedia", Logger);
        var query = await bot.Search(s);
        return query?.Search?.Select(s => s.Title).ToList();
    }

    [HttpGet]
    [Route("WantedFiles")]
    public async Task<IEnumerable<string?>?> WantedFilesAsync()
    {
        Logger.LogInformation("WantedFiles");
        Bot bot = new Bot(Configuration, "wiki", Logger);
        var files = await bot.WantedFiles(150);
        return files?.QueryPage?.Results?.Select(qp => qp.Title).ToList();
    }

    [HttpGet]
    [Route("WantedPages")]
    public async Task<IEnumerable<string?>?> WantedPagesAsync()
    {
        Logger.LogInformation("WantedPages");
        Bot bot = new Bot(Configuration, "wiki", Logger);
        var pages = await bot.WantedPages(450);
        return pages?.QueryPage?.Results?.Select(qp => qp.Title).ToList();
    }

    [HttpPost]
    [Route("WantedPages")]
    public async Task<bool> SaveWantedPagesAsync()
    {
        try
        {
            using var streamReader = new StreamReader(Request.Body);
            var page = streamReader.ReadToEndAsync().Result;

            Logger.LogInformation($"start downloading: {page}");

            Bot wikipediabot = new Bot(Configuration, "wikipedia", Logger);
            var wikipediapages = await wikipediabot.GetPage(page);

            Bot bot = new Bot(Configuration, "wiki", Logger);
            foreach (var item in wikipediapages!)
            {
                Logger.Log(LogLevel.Information, $"Title:{item.Title}, {string.IsNullOrEmpty(item.Revisions?.FirstOrDefault()?.Content)}");
                if (!string.IsNullOrEmpty(item.Title) && !string.IsNullOrEmpty(item.Revisions?.FirstOrDefault()?.Content))
                    await bot.Save(item.Title, item.Revisions!.FirstOrDefault()!.Content!);
                else
                    return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    [HttpPost]
    [Route("WantedFiles")]
    public async Task<bool> SaveWantedFilesAsync()
    {
        try
        {
            using var streamReader = new StreamReader(Request.Body);
            var page = streamReader.ReadToEndAsync().Result;
            Logger.LogInformation($"Wanted files {page}");
            Bot wikipediabot = new Bot(Configuration, "wikipedia", Logger);
            var wikipediaimages = await wikipediabot.GetFileInfo(true, page);

            Bot bot = new Bot(Configuration, "wiki", Logger);
            foreach (var item in wikipediaimages!)
            {
                if (!string.IsNullOrEmpty(item.Title) && (item.ImageInfo?.Length ?? 0) > 0)
                    await bot.UploadImage(item.Title, item.ImageInfo![0]!.ImageContent, item.ImageInfo![0]!.MediaType!);
                else
                    return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
            return false;
        }
    }

    [HttpPost]
    [Route("WantedResourcesTest")]
    public bool SaveWantedResourcesTestAsync()
    {
        using var streamReader = new StreamReader(Request.Body);
        var jsonStr = streamReader.ReadToEndAsync().Result;
        return jsonStr == "qwe";
    }
}
