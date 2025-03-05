using Core.Models;
using Infrastructure.DTOs;

namespace Infrastructure.Repository.IRepository
{
    public interface IBlockedCountryRepository
    {
        List<BlockedCountry> GetBlockedCountries(FilterBlockedCountriesDTO filter);
        ServiceResponseDTO BlockCountry(string countryCode);
        bool RemoveBlockedCountry(string countryCode);
        bool IsCountryBlocked(string countryCode);
        ServiceResponseDTO AddTemporalBlock(string countryCode, int durationMinutes);
    }
}
