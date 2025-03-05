using Infrastructure.Configuration;
using Infrastructure.DTOs;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Net;

namespace Infrastructure.Services.Implementation
{
    public class GeolocationService : IGeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly IpApiServiceSettings _settings;
        private readonly ILogger<GeolocationService> _logger;
        private readonly AsyncRetryPolicy<GeoInfoDTO> _retryPolicy;

        public GeolocationService(
            HttpClient httpClient,
            IOptions<IpApiServiceSettings> ipApiServiceSettings,
            ILogger<GeolocationService> logger)
        {
            _httpClient = httpClient;
            _settings = ipApiServiceSettings.Value;
            _logger = logger;

            // Configure retry policy
            _retryPolicy = Policy<GeoInfoDTO>
                .Handle<HttpRequestException>() // Network errors
                .OrResult(result => result == null) // API returned null
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogWarning(
                            $"Retry {retryAttempt} due to {outcome.Exception?.Message ?? "null response"}");
                    });
        }


        // Define retry policy for rate limits/transient errors

        public async Task<GeoInfoDTO> GetGeolocationAsync(string ipAddress)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync(
                    $"{_settings.BaseUrl}{ipAddress}?access_key={_settings.ApiKey}"
                );

                // Handle rate limits (status code 429)
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(30);
                    throw new HttpRequestException($"Rate limit exceeded. Retry after {retryAfter}");
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GeoInfoDTO>(json);
            });
        }
    }
}

