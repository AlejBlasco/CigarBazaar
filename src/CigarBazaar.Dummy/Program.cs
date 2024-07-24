using CigarBazaar.Application.CigarPrices;
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
                .BuildServiceProvider();

            var service = serviceProvider
                .GetService<ICigarPricesService>();

            var list = await service!.GetPriceListAsync();

        }
    }
}
