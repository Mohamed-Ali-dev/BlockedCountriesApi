using Core.Models;
using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Infrastructure.Services.Implementation;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Globalization;

namespace Infrastructure.Repository
{
    public class BlockedCountryRepository(RedisCacheService cacheService) : IBlockedCountryRepository
    {
        private  readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = LoadOrInitializeBlockedCountries(cacheService);
        private readonly RedisCacheService _cacheService = cacheService;
        private const string CacheKey = "BlockedCountriesCache";
        private const string TemporalBlocksKey = "temporal_blocks";

        private static ConcurrentDictionary<string, BlockedCountry> LoadOrInitializeBlockedCountries(RedisCacheService cacheService)
        {
            // Load from Redis
            var cachedData = cacheService.GetCacheData<ConcurrentDictionary<string, BlockedCountry>>(CacheKey);

            if (cachedData != null)
            {
                // Return existing data
                return cachedData;
            }

            // Initialize new dictionary and save to Redis
            var newDictionary = new ConcurrentDictionary<string, BlockedCountry>();
            cacheService.SetCachedData(CacheKey, newDictionary, TimeSpan.FromDays(1));
            return newDictionary;
        }



        public ServiceResponseDTO BlockCountry(string countryCode)
        {
             countryCode = countryCode.ToUpperInvariant(); 
            RegionInfo region;
            try
            {
                region = new RegionInfo(countryCode);
            }
            catch  { return new ServiceResponseDTO(false, "Something went wrong"); }

          

            BlockedCountry country = new BlockedCountry
            {
                CountryCode = countryCode,
                CountryName = region.EnglishName,
            };
            bool added = _blockedCountries.TryAdd(countryCode, country);
            if (added)
            {
                // Update the cache with the new dictionary
                _cacheService.SetCachedData(CacheKey, _blockedCountries, TimeSpan.FromDays(1));
                return new ServiceResponseDTO(true, $"Country code '{countryCode}' has been blocked.");
            }
            else
                return new ServiceResponseDTO(false, $"Country code '{countryCode}' is already blocked.");
        }

        public bool RemoveBlockedCountry(string countryCode)
        {
            countryCode = countryCode.ToUpperInvariant();
            bool removed = _blockedCountries.TryRemove(countryCode, out _);
            if (removed)
            {
                // Update the cache after removal
                _cacheService.SetCachedData(CacheKey, _blockedCountries, TimeSpan.FromDays(1));
            }
            return removed;
        }
        public List<BlockedCountry> GetBlockedCountries(FilterBlockedCountriesDTO filter)
        {
            var blockedCountries = _blockedCountries;
            IQueryable<BlockedCountry> query;
                query = _blockedCountries.Values.AsQueryable();
            if (!string.IsNullOrEmpty(filter.CountryName))
            {
                query = query.Where(x => x.CountryName.Contains(filter.CountryName, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(filter.CountryCode))
            {
                query = query.Where(x => x.CountryCode.Equals(filter.CountryCode, StringComparison.OrdinalIgnoreCase));
            }

            return query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
        }

        public bool IsCountryBlocked(string countryCode)
        {
            countryCode = countryCode.ToUpperInvariant();

            var filter = new FilterBlockedCountriesDTO { CountryCode = countryCode };
            var permanentBlockedCountries = GetBlockedCountries(filter);

            // Check temporary blocks
            var temporalBlockedCountries = _cacheService.GetCacheData<TemporalBlock>($"{TemporalBlocksKey}:{countryCode}");
            return (temporalBlockedCountries != null || permanentBlockedCountries.Any());

        }
        public ServiceResponseDTO AddTemporalBlock(string countryCode, int durationMinutes)
        {

            try
            {
                var _ = new RegionInfo(countryCode);
            }
            catch(ArgumentException ex) 
            {
                return new ServiceResponseDTO(false, "Invalid country code.");
            }

            if(durationMinutes < 1 || durationMinutes > 1440)
                return new ServiceResponseDTO(false, "Duration must be between 1 and 1440 minutes.");
            // Check existing blocks
            if (IsCountryBlocked(countryCode))
                return new ServiceResponseDTO(false, $"Country '{countryCode}' is already blocked");

            var block = new TemporalBlock
            {
                CountryCode = countryCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(durationMinutes)
            };

            _cacheService.SetCachedData($"{TemporalBlocksKey}:{countryCode}", block, TimeSpan.FromMinutes(durationMinutes));

            return new ServiceResponseDTO(true, $"Country '{countryCode}' temporarily blocked for {durationMinutes} minutes");
         
        }
    }

}
