using Microsoft.Extensions.Configuration;

namespace MediaWikiBot.Test
{
    public class BotFixture
    {
        public BotFixture() { 
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
        }
        public IConfiguration Configuration { get; internal set; }
    }
}