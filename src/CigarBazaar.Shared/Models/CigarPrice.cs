using Newtonsoft.Json;

namespace CigarBazaar.Shared.Models;

public class CigarPrice : Entity
{
    [JsonProperty(PropertyName = "price")]
    public decimal? Price { get; set; }

    [JsonProperty(PropertyName = "surchargedPrice")]
    public decimal? SurchargedPrice { get; set; }
}
