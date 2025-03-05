using Infrastructure.DTOs;

namespace Infrastructure.Services.Interfaces
{
    public interface IGeolocationService
    {
        Task<GeoInfoDTO> GetGeolocationAsync(string ipAddress);
    }
}
