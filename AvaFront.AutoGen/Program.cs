// get OpenAI Key and create config
using Microsoft.Extensions.Configuration;

namespace AvaFront.AutoGen
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            foreach (var item in configuration.AsEnumerable())
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value);
            }
            Console.WriteLine("Start AutoGen");
            //AutogenService autogenService = new AutogenService();
            //var conversations = await autogenService.ExecuteConversationAsync("Hello");
            //RestaurantAgent avaFrontAutoGenService = new AvaFrontAutoGenService();            
            
            //var conversations = await avaFrontAutoGenService.ExecuteConversationAsync("I'd like to make a reservation for Wednesday", null);
            Console.WriteLine("End AutoGen");
        }
    }
}
