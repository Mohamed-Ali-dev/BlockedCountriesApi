using Core.Models;
using Infrastructure.DTOs;

namespace Infrastructure.Repository.IRepository
{
    public interface ILogRepository
    {
        void AddLog(string ipAddress, string countryCode, bool blockedStatus, string userAgent);
        List<BlockedAttemptLog> GetAllLogs(PaginationDTO paginationDTO);
    }
}
