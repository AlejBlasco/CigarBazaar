using Newtonsoft.Json;

namespace CigarBazaar.Shared.Models;

public class CigarPrice
{
    [JsonProperty(PropertyName = "id")]
    public string Id
    {
        get { return _name.ToUpper().Trim().Replace(" ", "_"); }
    }

    [JsonProperty(PropertyName = "name")]
    public string Name
    {
        get { return _name; }
        set { _name = value ?? string.Empty; }
    }
    private string _name = string.Empty;

    [JsonProperty(PropertyName = "price")]
    public decimal? Price { get; set; }

    [JsonProperty(PropertyName = "surchargedPrice")]
    public decimal? SurchargedPrice { get; set; }
}
