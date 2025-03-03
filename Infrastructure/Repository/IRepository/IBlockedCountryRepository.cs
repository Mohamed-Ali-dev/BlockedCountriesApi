﻿using Core.Models;
using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.IRepository
{
    public interface IBlockedCountryRepository
    {
        List<BlockedCountry> GetBlockedCountries(FilterBlockedCountriesDTO filter);
        ServiceResponseDTO BlockCountry(string countryCode);
        bool RemoveBlockedCountry(string countryCode);
    }
}
