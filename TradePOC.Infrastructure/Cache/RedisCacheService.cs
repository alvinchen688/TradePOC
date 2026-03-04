using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TradePOC.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IJsonSerializer _jsonSerializer;

        public RedisCacheService(IDistributedCache cache, IJsonSerializer jsonSerializer)
        {
            _cache = cache;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);
            return string.IsNullOrEmpty(json) ? default : _jsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var json = _jsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration });
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var json = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(json);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
