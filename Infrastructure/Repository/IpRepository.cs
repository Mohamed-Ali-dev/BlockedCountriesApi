using Core.Services.Interfaces;
using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class IpRepository(IGeolocationService geolocationService) : IIpRepository
    {
        private readonly IGeolocationService _geolocationService = geolocationService;

        public async Task<GeoInfoDTO> GetCountryDetailsByIp(string ip)
        {
            var geeInfo = await _geolocationService.GetGeolocationAsync(ip);
            var capital = geeInfo.Location?.Capital?? "UnKnown Capital";
            
            return geeInfo;
        }
    }
}
