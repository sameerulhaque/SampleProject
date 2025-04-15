using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Newtonsoft.Json;
using SampleProject.Infrastructure.Tenant;
using StackExchange.Redis;

namespace SampleProject.Infrastructure.Caching
{
    public interface IRedisCacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, int? expiry = null);
        Task RemoveAsync(string key, bool isPrefix = false);
        Task<bool> KeyExistsAsync(string key);

    }

    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _redisDatabase;
        private readonly ITenantService _tenantService;

        public RedisCacheService(IConnectionMultiplexer redisConnection, ITenantService tenantService)
        {
            _redisDatabase = redisConnection.GetDatabase();
            _tenantService = tenantService;
        }
        public async Task<T> GetAsync<T>(string key)
        {
            var tenantKey = _tenantService.GetTenantKey(key);
            var value = await _redisDatabase.StringGetAsync(tenantKey);
            return value.HasValue ? JsonConvert.DeserializeObject<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, int? seconds = null)
        {
            var expiry = CacheHelper.GetCacheExpirationTimeSpan(seconds ?? 0);
            var tenantKey = _tenantService.GetTenantKey(key);
            var serializedValue = JsonConvert.SerializeObject(value);
            await _redisDatabase.StringSetAsync(tenantKey, serializedValue, expiry);
        }
        public async Task RemoveAsync(string key, bool isPrefix = false)
        {
            var tenantKey = _tenantService.GetTenantKey(key);

            if (!isPrefix)
            {
                await _redisDatabase.KeyDeleteAsync(tenantKey);
            }
            else
            {
                var server = _redisDatabase.Multiplexer.GetServer(_redisDatabase.Multiplexer.GetEndPoints()[0]);
                var keys = server.Keys(pattern: $"{tenantKey + "_"}*").ToArray();
                if (keys.Any())
                {
                    await _redisDatabase.KeyDeleteAsync(keys);
                }
            }
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            var tenantKey = _tenantService.GetTenantKey(key);
            return await _redisDatabase.KeyExistsAsync(tenantKey);
        }

    }
}
