using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ServiceStack.Redis.Generic;
using StackExchange.Redis;
using WordlersAPI.Interfaces;

namespace WordlersAPI.Services
{
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IDatabase _redis;

        public DistributedCacheService(IDistributedCache cache, IConnectionMultiplexer redis)
        {
            _cache = cache;
            _redis = redis.GetDatabase();
        }

        public T Get<T>(string key)
        {
            var value = _cache.GetString(key);

            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public T Set<T>(string key, T value, TimeSpan time)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = time
            };

            _cache.SetString(key, JsonConvert.SerializeObject(value), options);

            return value;
        }

        public async Task<bool> AddToStore(string key, string value)
        {
           return await _redis.SetAddAsync(key, value);
        }

        public async Task<bool> IsStoreContains(string key, string value)
        {
            return await _redis.SetContainsAsync(key, value);    
        }

        public async Task ClearStore(string key)
        {
            await _redis.KeyDeleteAsync(key); 
        }

        public async Task<long> GetCounter(string key)
        {
           return await _redis.HashIncrementAsync(key, "counter", 1);
        }
    }
}
