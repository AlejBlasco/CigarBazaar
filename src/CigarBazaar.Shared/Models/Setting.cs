using Newtonsoft.Json;

namespace CigarBazaar.Shared.Models;

public class Setting : Entity
{
    [JsonProperty(PropertyName = "lastPricesUpdate")]
    public DateTime? LastPricesUpdate { get; private set; }

    public Setting(DateTime? lastPricesUpdate)
    {
        this.Name = "Cigar Bazaar Settings";
        LastPricesUpdate = lastPricesUpdate ?? DateTime.Now;
    }
}
