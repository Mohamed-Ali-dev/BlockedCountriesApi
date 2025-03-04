using Core.Models;
using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Infrastructure.Services.Implementation;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Globalization;

namespace Infrastructure.Repository
{
    public class BlockedCountryRepository(RedisCacheService cacheService, IHttpContextAccessor httpContextAccessor) : IBlockedCountryRepository
    {
        private  readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = LoadOrInitializeBlockedCountries(cacheService);
        private readonly RedisCacheService _cacheService = cacheService;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        private const string CacheKey = "BlockedCountriesCache";

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
            var blockedCountries = _cacheService.GetCacheData<ConcurrentDictionary<string, BlockedCountry>>(CacheKey);
            IQueryable<BlockedCountry> query;
            if (blockedCountries is null)
            {
                _cacheService.SetCachedData(CacheKey, _blockedCountries, TimeSpan.FromDays(1));
                query = _blockedCountries.Values.AsQueryable();
            }
            else
            {
                query = blockedCountries.Values.AsQueryable();
            }
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
    }

}
