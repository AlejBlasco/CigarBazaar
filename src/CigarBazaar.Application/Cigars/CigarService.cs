using CigarBazaar.Application.CigarPrices;
using CigarBazaar.Shared.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CigarBazaar.Application.Cigars;

public class CigarService : ICigarService
{
    private readonly IMemoryCache memoryCache;
    private readonly ICigarPricesService cigarPricesService;
    private readonly MemoryCacheEntryOptions cacheEntryOptions;

    public CigarService(IMemoryCache memoryCache, ICigarPricesService cigarPricesService)
    {
        this.memoryCache = memoryCache
            ?? throw new ArgumentNullException(nameof(memoryCache));

        this.cigarPricesService = cigarPricesService
            ?? throw new ArgumentNullException(nameof(cigarPricesService));

        cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(5));
    }

    public async Task<IList<Cigar>> GetCigarsAsync()
    {
        memoryCache.TryGetValue("PricesUpdateDate", out DateTime? cigarsDate);

        if (cigarsDate == null)
            return await SetCigarsCacheAync();

        var lastUpdateDate = await cigarPricesService.GetPricesUpdateDate();

        if (cigarsDate >= lastUpdateDate)
        {
            memoryCache.TryGetValue("CigarsList", out IList<Cigar>? cigarList);
            return cigarList ?? new List<Cigar>();
        }
        else
            return await SetCigarsCacheAync();
    }

    private async Task<IList<Cigar>> SetCigarsCacheAync()
    {
        var data = await cigarPricesService.GetPriceListAsync();
        if (data != null)
            return memoryCache.Set<IList<Cigar>>("CigarsList", data.Cigars, cacheEntryOptions);
        else 
            return new List<Cigar>();

    }
}
