using CigarBazaar.Shared.Models;
using PuppeteerSharp;

namespace CigarBazaar.Application.CigarPrices;

public interface ICigarPricesService
{
    Task<DateTime?> GetPricesUpdateDate();

    Task<CigarPriceList> GetPriceListAsync();
}
