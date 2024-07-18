using CigarBazaar.Application.Hacienda;
using CigarBazaar.Infrastructure.Repository;
using CigarBazaar.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;

namespace CigarBazaar.Dummy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

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

            GenerateJsonFiles(config, priceList, lastUpdate);
            Console.ReadKey();
        }

        private static void GenerateJsonFiles(IConfiguration config, IList<CigarPrice>? cigarPrices, DateTime? scrappingDate)
        {
            // Check settings.
            Setting settings = new Setting(new DateTime(1900, 12, 31));
            if (System.IO.File.Exists(config["JsonData:Settings"]))
            {
                using (var sr = new StreamReader(config["JsonData:Settings"]))
                {
                    settings = JsonConvert.DeserializeObject<Setting>(sr.ReadToEnd());
                }
            }

            // Write prices.json
            if (cigarPrices != null && settings!.LastPricesUpdate < (scrappingDate ?? DateTime.Now))
            {
                Console.WriteLine("Saving price list to JSON");
                var priceSerialized = JsonConvert.SerializeObject(cigarPrices);
                using (var sw = new StreamWriter(config["JsonData:Prices"]))
                {
                    sw.WriteLine(priceSerialized);
                }
                Console.WriteLine($"JSON generated");
            }

            // Write settings.json
            Console.WriteLine("Saving Settings to JSON");
            settings = new Setting(scrappingDate);
            var settingsSerialized = JsonConvert.SerializeObject(settings);
            using (var sw = new StreamWriter(config["JsonData:Settings"]))
            {
                sw.WriteLine(settingsSerialized);
            }
            Console.WriteLine($"JSON generated");
        }

        private static async Task UpdateCosmosDB(IConfiguration config, IList<CigarPrice>? cigarPrices)
        {
            var accountEndpoint = config["CosmosDb:AccountEndpoint"];
            var authKey = config["CosmosDb:AuthKey"];
            var databaseId = config["CosmosDb:DatabaseId"];
            var cigarContainerId = config["CosmosDb:CigarContainerId"];

            var cigarPriceRepository = new CosmosRepository<CigarPrice>(accountEndpoint,
                authKey,
                databaseId,
                cigarContainerId);

            Console.WriteLine("Saving price list to JSON");
            if (cigarPrices != null)
            {
                foreach (var item in cigarPrices!)
                {
                    await cigarPriceRepository.UpsertItemAsync(item);
                }
            }

            //TODO: Add last downlaod.

        }

    }
}
