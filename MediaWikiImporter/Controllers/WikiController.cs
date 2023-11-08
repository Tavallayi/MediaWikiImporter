using MediaWikiBot;
using Microsoft.AspNetCore.Mvc;

namespace Project1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WikiController : ControllerBase
{
    public WikiController(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    [HttpGet]
    [Route("WantedFiles")]
    public async Task<IEnumerable<string?>?> WantedFilesAsync()
    {
        Bot bot = new Bot(Configuration, "wiki");
        var files = await bot.WantedFiles(15);
        return files?.QueryPage?.Results?.Select(qp => qp.Title).ToList();
    }

    [HttpGet]
    [Route("WantedPages")]
    public async Task<IEnumerable<string?>?> WantedPagesAsync()
    {
        Bot bot = new Bot(Configuration, "wiki");
        var pages = await bot.WantedPages(50);
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

            Bot wikipediabot = new Bot(Configuration, "wikipedia");
            var wikipediapages = await wikipediabot.GetPage(page);

            Bot bot = new Bot(Configuration, "wiki");
            foreach (var item in wikipediapages!)
            {
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

            Bot wikipediabot = new Bot(Configuration, "wikipedia");
            var wikipediaimages = await wikipediabot.GetFileInfo(true, page);

            Bot bot = new Bot(Configuration, "wiki");
            foreach (var item in wikipediaimages!)
            {
                if (!string.IsNullOrEmpty(item.Title) && (item.ImageInfo?.Length ?? 0) > 0)
                    await bot.UploadImage(item.Title, item.ImageInfo![0]!.ImageContent, item.ImageInfo![0]!.MediaType!);
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
    [Route("WantedResourcesTest")]
    public bool SaveWantedResourcesTestAsync()
    {
        using var streamReader = new StreamReader(Request.Body);
        var jsonStr = streamReader.ReadToEndAsync().Result;
        return jsonStr == "qwe";
    }
}
