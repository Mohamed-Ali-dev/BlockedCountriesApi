using Infrastructure.DTOs;

namespace Infrastructure.Repository.IRepository
{
    public interface IIpRepository
    {
        Task<GeoInfoDTO> GetCountryDetailsByIp(string ip);
    }
}
