using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Services.Implementation
{
    public class RedisCacheService(IDistributedCache? cache)
    {

        public T GetCacheData<T>(string key)
        {
            var jsonData = cache.GetString(key);

            if (jsonData is null)
                return default;

            return JsonConvert.DeserializeObject<T>(jsonData)!;
        }
        public void SetCachedData<T>(string Key, T data, TimeSpan cacheDuration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
            var jsonData = JsonConvert.SerializeObject(data);

            cache.SetString(Key, jsonData, options);
        }
        public void RemoveCache(string key) => cache.Remove(key);
    }
}
