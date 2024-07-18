using CigarBazaar.Shared.Models;

namespace CigarBazaar.Application.CigarPrices;

public interface ICigarPricesService
{
    Task<List<CigarPrice>> GetPricesFromJsonAsync(string path, CancellationToken cancellationToken = default);
}
