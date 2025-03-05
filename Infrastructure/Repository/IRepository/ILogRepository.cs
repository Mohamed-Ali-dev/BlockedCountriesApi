using Core.Models;
using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.IRepository
{
    public interface ILogRepository
    {
        void AddLog(string ipAddress, string countryCode, bool blockedStatus, string userAgent);
        List<BlockedAttemptLog> GetAllLogs(PaginationDTO paginationDTO);
    }
}
