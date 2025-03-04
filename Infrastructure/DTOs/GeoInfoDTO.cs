using Newtonsoft.Json;

namespace Infrastructure.DTOs
{
    public class GeoInfoDTO
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_name")]
        public string Country { get; set; }
    }
}
