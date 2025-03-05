using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Repository
{
    public class IpRepository(IGeolocationService geolocationService) : IIpRepository
    {

        public async Task<GeoInfoDTO> GetCountryDetailsByIp(string ip)
        {
            var geeInfo = await geolocationService.GetGeolocationAsync(ip);
            var capital = geeInfo.Location?.Capital?? "UnKnown Capital";
            
            return geeInfo;
        }
    }
}
