using Core.Models;
using Infrastructure.DTOs;
using Infrastructure.Repository.IRepository;
using Infrastructure.Services.Implementation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class LogRepository(RedisCacheService cacheService) : ILogRepository
    {
        private const string _logsCacheKey = "blocked-attempts-logs";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(1);
        private readonly RedisCacheService _cacheService = cacheService;

        private List<BlockedAttemptLog> _logEntries = LoadOrInitializeLogs(cacheService);
        private static List<BlockedAttemptLog> LoadOrInitializeLogs(RedisCacheService cacheService)
        {
            // Load from Redis
            var cachedData = cacheService.GetCacheData<List<BlockedAttemptLog>>(_logsCacheKey);

            if (cachedData != null)
            {
                // Return existing data
                return cachedData;
            }

            // Initialize new list and save to Redis
            var newList = new List<BlockedAttemptLog>();
            cacheService.SetCachedData(_logsCacheKey, newList, TimeSpan.FromDays(1));
            return newList;
        }
        public void AddLog(string ipAddress, string countryCode, bool blockedStatus, string userAgent)
        {
            var log = new BlockedAttemptLog
            {
                IpAddress = ipAddress,
                CountryCode = countryCode,
                BlockedStatus = blockedStatus,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };
         _logEntries.Add(log);

         _cacheService.SetCachedData(_logsCacheKey, _logEntries, _cacheDuration);
        }

        public List<BlockedAttemptLog> GetAllLogs(PaginationDTO paginationDTO)
        {
          return _logEntries.OrderBy(x => x.Timestamp).Skip((paginationDTO.Page - 1) * paginationDTO.PageSize).Take(paginationDTO.PageSize).ToList();
        }
    }
}
