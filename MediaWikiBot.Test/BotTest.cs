using FluentAssertions;

namespace MediaWikiBot.Test;

public class BotTest : IClassFixture<BotFixture>
{
    public BotTest(BotFixture botFixture)
    {
        BotFixture = botFixture;
    }

    public BotFixture BotFixture { get; }

    [Fact]
    public void TestInitInvalidBot()
    {
        Action act = () => { Bot bot = new(BotFixture.Configuration, "wiki1"); };

        act.Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void TestInitBotWithInvalidApi()
    {
        Action act = () => { Bot bot = new(BotFixture.Configuration, "invalidwikipedia"); };

        act.Should().Throw<InvalidOperationException>().WithMessage("Invalid api.");
    }

    [Theory]
    [InlineData("wiki")]
    [InlineData("wikipedia")]
    public void TestInitBot(string wiki)
    {
        Action act = () => { Bot bot = new(BotFixture.Configuration, wiki); };

        act.Should().NotThrow<InvalidDataException>();
    }

    [Fact]
    public async void TestLoginSuccess()
    {
        Func<Task<Bot>> login = async () =>
        {
            Bot bot = new(BotFixture.Configuration, "wiki");
            await bot.Login();
            return bot;
        };

        var bot = await login.Should().NotThrowAsync();

        bot.Subject.LoginToken.Should().NotBeNullOrEmpty();
        bot.Subject.CsrfToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async void TestLoginFailed()
    {
        Func<Task<Bot>> login = async () =>
        {
            Bot bot = new(BotFixture.Configuration, "wikipedia");
            await bot.Login();
            return bot;
        };

        await login.Should().ThrowAsync<InvalidOperationException>().WithMessage("Invalid username or password.");
    }

    [Fact]
    public async void TestReadPageContent()
    {
        Func<Task<Page?>> action = async () =>
        {
            Bot bot = new(BotFixture.Configuration, "wiki");
            var page = await bot.GetPage("Main Page");
            return page?.FirstOrDefault();
        };

        var result = await action.Should().NotThrowAsync();
        result.Subject.Should().NotBeNull();
        var p = result.Subject;

        p!.Revisions?.FirstOrDefault().Should().NotBeNull();
        var rev = p!.Revisions?.FirstOrDefault();
        rev!.Content.Should().Be("<strong>MediaWiki has been installed.</strong>\n\nConsult the [https://www.mediawiki.org/wiki/Special:MyLanguage/Help:Contents User's Guide] for information on using the wiki software.\n\n== Getting started ==\n* [https://www.mediawiki.org/wiki/Special:MyLanguage/Manual:Configuration_settings Configuration settings list]\n* [https://www.mediawiki.org/wiki/Special:MyLanguage/Manual:FAQ MediaWiki FAQ]\n* [https://lists.wikimedia.org/postorius/lists/mediawiki-announce.lists.wikimedia.org/ MediaWiki release mailing list]\n* [https://www.mediawiki.org/wiki/Special:MyLanguage/Localisation#Translation_resources Localise MediaWiki for your language]\n* [https://www.mediawiki.org/wiki/Special:MyLanguage/Manual:Combating_spam Learn how to combat spam on your wiki]");
    }

    [Fact]
    public async void TestEditPage()
    {
        Bot bot = new(BotFixture.Configuration, "wiki");
        Func<Task> action = async () =>
        {
            await bot.Save("test", "This is test");
        };

        await action.Should().NotThrowAsync();
        var pages = await bot.GetPage("test");
        pages.Should().NotBeNullOrEmpty();
        var page = pages?.First();
        page?.Revisions?.Should().NotBeNullOrEmpty();
        var rev = page?.Revisions?.First();
        rev?.Content.Should().Be("This is test");
    }

    [Fact]
    public async void TestImportPageFromWikipedia()
    {
        Bot wikipediaBot = new(BotFixture.Configuration, "wikipedia");
        var pages = await wikipediaBot.GetPage("دریا");
        pages.Should().NotBeNullOrEmpty();

        Bot bot = new(BotFixture.Configuration, "wiki");
        await bot.Save(pages?.First()?.Title!, pages!.First()!.Revisions!.First()!.Content!);
        var marinPage = await bot.GetPage("دریا");
        marinPage.Should().NotBeNullOrEmpty();
        marinPage!.First()!.Revisions.Should().NotBeNullOrEmpty();
        marinPage!.First()!.Revisions!.First()!.Content.Should().Be(pages!.First()!.Revisions!.First()!.Content!);
    }

    [Fact]
    public async void TestGetImageInfo()
    {
        await ReadWikipediaImages();
    }

    private async Task<Page[]?> ReadWikipediaImages()
    {
        Bot wikipediaBot = new(BotFixture.Configuration, "wikipedia");

        Func<Task<Page[]?>> action = async () =>
        {
            return await wikipediaBot.GetFileInfo(true, "پرونده:View_towards_the_sea_from_the_coastal_path_on_Guanyinbi.jpg", "پرونده:Arabian Sea - n22e70.jpg");
        };

        var ret = await action.Should().NotThrowAsync();
        ret.Subject.Should().NotBeNullOrEmpty();
        return ret.Subject;
    }

    [Fact]
    public async void TestUploadImage()
    {
        var images = await ReadWikipediaImages();
        Bot wiki = new(BotFixture.Configuration, "wiki");
        foreach (var item in images!)
        {
            foreach (var ii in item.ImageInfo!)
            {
                Func<Task> act = async () =>
                {
                    await wiki.UploadImage(item.Title!, ii.ImageContent, ii.Mime!);
                };
                await act.Should().NotThrowAsync();
            }
        }

    }

    [Fact]
    public async void TestWantedPages()
    {
        Bot bot = new(BotFixture.Configuration, "wiki");
        Func<Task<Query?>> act = async () => await bot.WantedPages(10);
        var ret = await act.Should().NotThrowAsync();
        var query = ret.Subject;
        query.Should().NotBeNull();
        query!.QueryPage.Should().NotBeNull();
        query!.QueryPage!.Results.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async void TestWantedFiles()
    {
        Bot bot = new(BotFixture.Configuration, "wiki");
        Func<Task<Query?>> act = async () => await bot.WantedFiles(10);
        var ret = await act.Should().NotThrowAsync();
        var query = ret.Subject;
        query.Should().NotBeNull();
        query!.QueryPage.Should().NotBeNull();
        query!.QueryPage!.Results.Should().NotBeNullOrEmpty();
    }
}