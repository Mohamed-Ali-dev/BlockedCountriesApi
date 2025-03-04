using Infrastructure.DTOs;

namespace Core.Services.Interfaces
{
    public interface IGeolocationService
    {
        Task<GeoInfoDTO> GetGeolocationAsync(string ipAddress);
    }
}
