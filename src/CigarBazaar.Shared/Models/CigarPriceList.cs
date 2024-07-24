namespace CigarBazaar.Shared.Models;

public class CigarPriceList
{
    public DateTime PricesUpdateDate { get; set; }

    public IList<Cigar> Cigars { get; set; } = new List<Cigar>();
}
