using Core.Helper;
using Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Implementation
{
    public class GeolocationService(HttpClient httpClient, IOptions<IpApiServiceSettings> ipApiServiceSettings) : IGeolocationService
    {
        public async Task<GeoInfo> GetGeolocationAsync(string ipAddress)
        {
            try
            {
                var response = await httpClient.GetAsync($"{ipApiServiceSettings.Value.BaseUrl}{ipAddress}?access_key={ipApiServiceSettings.Value.ApiKey}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var geoInfo = JsonConvert.DeserializeObject<GeoInfo>(json);
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
