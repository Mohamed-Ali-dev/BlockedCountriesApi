using Infrastructure.Repository.IRepository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    internal class BlockedCountryRepository : IBlockedCountryRepository
    {
        private static readonly ConcurrentDictionary<string, bool> _blockedCountries =
        new();
        public bool blockCountry(string countryCode)
        {
            return _blockedCountries.TryAdd(countryCode.ToUpper(), true);
        }
    }
}
