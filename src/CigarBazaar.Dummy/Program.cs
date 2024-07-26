using CigarBazaar.Application.CigarPrices;
using CigarBazaar.Application.Cigars;
using CigarBazaar.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CigarBazaar.Dummy
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            //setup our DI
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

            IConfiguration config = builder.Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<CigarPricesConfiguration>(new CigarPricesConfiguration { UrlToScrap = config["CigarPricesConfiguration:UrlToScrap"] })
                .AddSingleton<ICigarPricesService, CigarPricesService>()
                .AddSingleton<ICigarService, CigarService>()
                .BuildServiceProvider();

            // Display the menu
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Test ICigarPricesService -> GetPriceListAsync");
            Console.WriteLine("2. Test ICigarService -> GetCigarsAsync");
            Console.WriteLine("3. Exit");
            string input = Console.ReadLine() ?? string.Empty;

            if (input.Equals("1"))
            {
                var cigarPricesService = serviceProvider
                    .GetService<ICigarPricesService>();

                await cigarPricesService!.GetPriceListAsync();
            }
            else if (input.Equals("2"))
            {
                var cigarService = serviceProvider
                    .GetService<ICigarService>();

                await cigarService!.GetCigarsAsync();
            }
            else
                Console.WriteLine("Exiting...");

        }
    }
}
