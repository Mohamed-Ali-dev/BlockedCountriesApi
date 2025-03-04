using Core.Services.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Services.Implementation
{
    public class GeolocationService(HttpClient httpClient, IOptions<IpApiServiceSettings> ipApiServiceSettings) : IGeolocationService
    {
        public async Task<GeoInfoDTO> GetGeolocationAsync(string ipAddress)
        {
            try
            {
                var response = await httpClient.GetAsync($"{ipApiServiceSettings.Value.BaseUrl}{ipAddress}?access_key={ipApiServiceSettings.Value.ApiKey}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var geoInfo = JsonConvert.DeserializeObject<GeoInfoDTO>(json);
                return geoInfo;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
                return null;
            }
        }
    }
}
