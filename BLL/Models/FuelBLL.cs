using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class FuelBLL
    {
        [JsonProperty(PropertyName = "gas(euro/MWh)")]
        public double Gas { get; set; }
        [JsonProperty(PropertyName = "kerosine(euro/MWh)")]
        public double Kerosine { get; set; }
        [JsonProperty(PropertyName = "co2(euro/ton)")]
        public decimal Co2 { get; set; }
        [JsonProperty(PropertyName = "wind(%)")]
        public double Wind { get; set; }
    }
}
