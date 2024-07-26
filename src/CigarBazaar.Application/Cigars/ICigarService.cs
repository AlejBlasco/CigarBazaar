using CigarBazaar.Shared.Models;

namespace CigarBazaar.Application.Cigars;

public interface ICigarService
{
    Task<IList<Cigar>> GetCigarsAsync();
}
