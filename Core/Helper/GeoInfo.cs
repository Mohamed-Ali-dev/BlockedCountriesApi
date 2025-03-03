using Newtonsoft.Json;

namespace Core.Helper
{
    public class GeoInfo
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_name")]
        public string Country { get; set; }
    }
}
