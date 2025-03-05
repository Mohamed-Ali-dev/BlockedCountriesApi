using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Services.Implementation
{
    public class RedisCacheService
    {
        private readonly IDistributedCache? _cache;
        public RedisCacheService(IDistributedCache? cache)
        {
            _cache = cache;
        }
        public async Task<T> GetCacheData<T>(string key)
        {
            var jsonData = await _cache.GetStringAsync(key);

            if (jsonData is null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(jsonData)!;
        }
        public async Task SetCachedData<T>(string Key, T data, TimeSpan cacheDuration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
            var jsonData = JsonConvert.SerializeObject(data);

          await  _cache.SetStringAsync(Key, jsonData, options);
        }
        public async Task RemoveCache(string key) => await _cache.RemoveAsync(key);
    }
}
