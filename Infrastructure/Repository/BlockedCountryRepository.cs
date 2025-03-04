using Core.Models;
using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class BlockedCountryRepository : IBlockedCountryRepository
    {
        private static readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = new();
        public ServiceResponseDTO blockCountry(string countryCode)
        {
             countryCode = countryCode.ToUpperInvariant(); 
            RegionInfo region;
            try
            {
                region = new RegionInfo(countryCode);
            }
            catch (Exception e) {

            return new ServiceResponseDTO(false, "Something went wrong");
            }
            var countryName = region.EnglishName;

            BlockedCountry country = new BlockedCountry
            {
                CountryCode = countryCode,
                CountryName = countryName,
            };
            return _blockedCountries.TryAdd(countryCode, country)? new ServiceResponseDTO(true, $"Country code '{countryCode}' has been blocked.") 
                : new ServiceResponseDTO(false, $"Country code '{countryCode} is already blocked'") ;

        }

        public bool RemoveBlockedCountry(string countryCode)
        {
             countryCode = countryCode.ToUpperInvariant();
            return _blockedCountries.Remove(countryCode, out _);
        }
        public List<BlockedCountry> GetBlockedCountries()
        {
            return _blockedCountries.Values.ToList();
        }

    }
}
