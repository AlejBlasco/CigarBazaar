using CigarBazaar.Shared.Models;
using Newtonsoft.Json;

namespace CigarBazaar.Application.CigarPrices;

public class CigarPricesService : ICigarPricesService
{
    public async Task<List<CigarPrice>> GetPricesFromJsonAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        List<CigarPrice> prices = new List<CigarPrice>();

        await Task.Run(() =>
        {
            prices = JsonConvert.DeserializeObject<List<CigarPrice>?>(File.ReadAllText(path)) ?? new List<CigarPrice>();
        }, cancellationToken);

        return prices;
    }
}
