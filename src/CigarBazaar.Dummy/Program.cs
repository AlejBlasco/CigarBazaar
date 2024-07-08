using CigarBazaar.Application.Hacienda;
using Microsoft.Extensions.Configuration;

namespace CigarBazaar.Dummy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var urlToScrap = config["PriceOfLaborService:UrlToScrap"];

            if (string.IsNullOrEmpty(urlToScrap))
                throw new ArgumentNullException(nameof(urlToScrap));
            
            IPriceOfLaborService priceOfLaborService = new PriceOfLaborService(urlToScrap);

            Console.WriteLine("Getting last update date");
            var lastUpdate = await priceOfLaborService.GetLastUpdateDateAsync();
            Console.WriteLine($"Last update on {lastUpdate.ToString() ?? "NULL"}");

            Console.WriteLine("Getting price list");
            var priceList = await priceOfLaborService.GetPriceOfLaborListAsync();
            Console.WriteLine($"Retrieved {priceList.Count} records");

            Console.ReadKey();
        }


    }
}
