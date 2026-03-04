using Microsoft.Extensions.Caching.Memory;

namespace TradePOC.Infrastructure.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IJsonSerializer _jsonSerializer;

        public MemoryCacheService(IMemoryCache memoryCache, IJsonSerializer jsonSerializer)
        {
            _memoryCache = memoryCache;
            _jsonSerializer = jsonSerializer;
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out string json))
            {
                return Task.FromResult(string.IsNullOrEmpty(json) ? default : _jsonSerializer.Deserialize<T>(json));
            }
            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var json = _jsonSerializer.Serialize(value);
            _memoryCache.Set(key, json, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration });
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key) => Task.FromResult(_memoryCache.TryGetValue(key, out _));
        public Task RemoveAsync(string key) { _memoryCache.Remove(key); return Task.CompletedTask; }
    }
}
