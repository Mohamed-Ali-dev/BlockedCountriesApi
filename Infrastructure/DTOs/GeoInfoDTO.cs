using Newtonsoft.Json;

namespace Infrastructure.DTOs
{
    public class GeoInfoDTO
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_name")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("continent_name")]
        public string Continent { get; set; }
        [JsonProperty("location")]
        public Location Location { get; set; }
    }
    public class Location
    {
        [JsonProperty("capital")]

        public string Capital { get; set; }
    }
}
