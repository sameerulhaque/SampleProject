using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using SampleProject.Infrastructure.Tenant;
using StackExchange.Redis;


namespace SampleProject.Infrastructure.Caching
{
    public interface IInMemoryCacheService
    {
        void Set<T>(string cacheKey, T objectToCache, int expireCacheInSecs);
        T Get<T>(string cacheKey);
        void Remove(string cacheKey, bool isPrefix = false);
        bool KeyExists(string cacheKey);
    }


    public class InMemoryCacheService : IInMemoryCacheService
    {
        private readonly ITenantService _tenantService;

        public InMemoryCacheService(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        private static CacheItemPolicy GetInMemoryCacheItemPolicy(int absoluteExpirationInSeconds)
        {
            CacheItemPolicy cip = new CacheItemPolicy();
            try
            {
                cip.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddSeconds(absoluteExpirationInSeconds));
            }
            catch (Exception)
            {
                throw;
            }
            return cip;
        }

        public void Set<T>(string cacheKey, T objectToCache, int expireCacheInSecs)
        {
            cacheKey = _tenantService.GetTenantKey(cacheKey);
            MemoryCache.Default.Set(new CacheItem(cacheKey, objectToCache), GetInMemoryCacheItemPolicy(expireCacheInSecs));
        }

        public T Get<T>(string cacheKey)
        {
            cacheKey = _tenantService.GetTenantKey(cacheKey);
            return (T)MemoryCache.Default[cacheKey];
        }
        public void Remove(string cacheKey, bool isPrefix = false)
        {
            if (!isPrefix)
            {
                if (KeyExists(cacheKey))
                {
                    cacheKey = _tenantService.GetTenantKey(cacheKey);
                    MemoryCache.Default.Remove(cacheKey);
                }
            }
            else
            {
                var keysToRemove = MemoryCache.Default.Select(kvp => kvp.Key)
                                                       .Where(key => key.StartsWith(cacheKey))
                                                       .ToList();
                foreach (var key in keysToRemove)
                {
                    MemoryCache.Default.Remove(key);
                }
            }
        }

        public bool KeyExists(string cacheKey)
        {
            cacheKey = _tenantService.GetTenantKey(cacheKey); 

            return MemoryCache.Default.Contains(cacheKey);
        }

    }
}
