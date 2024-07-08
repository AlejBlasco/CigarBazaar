using CigarBazaar.Shared.Models;

namespace CigarBazaar.Application.Hacienda;

public interface IPriceOfLaborService
{
    Task<DateTime?> GetLastUpdateDateAsync();

    Task<IList<CigarPrice>> GetPriceOfLaborListAsync();
}
