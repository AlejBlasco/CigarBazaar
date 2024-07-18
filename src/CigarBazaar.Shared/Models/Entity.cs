using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CigarBazaar.Shared.Models
{
    public class Entity
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
    }
}
