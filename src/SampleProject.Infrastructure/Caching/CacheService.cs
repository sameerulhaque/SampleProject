
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace SampleProject.Infrastructure.Caching
{
    public interface ICacheService
    {
        Task<T> GetDataAsync<T>(Func<Task<T>> codeBlock, string cacheKey, IDictionary<string, object> paramWithValues, int refreshCacheDuration = 30, string cacheType = "inmemory", int cacheDuration = 180);
        bool RemoveByCondition(Func<string, bool> predicate);
    }

    public class CacheService : ICacheService
    {
        //private readonly IRedisCacheService _redisCacheService;
        private readonly IInMemoryCacheService _inmemoryCacheService;
        private readonly bool _useCaching;

        private static ConcurrentDictionary<string, object> _cacheLocks = new ConcurrentDictionary<string, object>();
        private static ConcurrentDictionary<string, int> _refreshFlags = new ConcurrentDictionary<string, int>();

        public CacheService(/*IRedisCacheService redisCacheService,*/
                            IInMemoryCacheService inmemoryCacheService,
                            IConfiguration configuration)
        {
            //_redisCacheService = redisCacheService;
            _inmemoryCacheService = inmemoryCacheService;
            _useCaching = true;// configuration.GetValue<bool?>("CacheSettings:UseCaching") ?? true;
        }

        public bool RemoveByCondition(Func<string, bool> predicate)
        {
            var cacheKeysToRemove = _refreshFlags.Keys.Where(predicate);
            foreach (var key in cacheKeysToRemove)
            {
                _refreshFlags.TryRemove(key, out _);
                _inmemoryCacheService.Remove(key);
            }
            return true;
        }
        public async Task<T> GetDataAsync<T>(Func<Task<T>> codeBlock, string cacheKey, IDictionary<string, object> paramWithValues, int refreshCacheDuration = 30, string cacheType = "inmemory", int cacheDuration = 180)
        {
            cacheDuration = refreshCacheDuration > cacheDuration ? refreshCacheDuration * 3 : cacheDuration;

            var hash = "";
            if (paramWithValues.Count > 0)
            {
                var paramWithValuesString = string.Join(",", paramWithValues.OrderBy(x => x.Key).Select(p => $"{p.Key}={p.Value}"));
                hash = Convert.ToBase64String(MD5.HashData(Encoding.UTF8.GetBytes(paramWithValuesString.ToString())));
            }
            cacheKey = $"{cacheKey}_{hash}_{cacheType}";
            string refreshKey = $"{cacheKey}_refresh";

            if (_useCaching && cacheDuration > 0)
            {

                object lockObj = _cacheLocks.GetOrAdd(cacheKey, new object());

                // If cache exists, return it and optionally trigger async refresh
                if (_inmemoryCacheService.KeyExists(cacheKey))
                {
                    T cachedData = _inmemoryCacheService.Get<T>(cacheKey);

                    if (!_inmemoryCacheService.KeyExists(refreshKey) && _refreshFlags.TryGetValue(cacheKey, out int flag) && flag == 0)
                    {
                        _refreshFlags[cacheKey] = 1;
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                T freshData = await Task.Run(codeBlock);
                                if (freshData != null)
                                {
                                    _inmemoryCacheService.Set(cacheKey, freshData, cacheDuration);
                                    _inmemoryCacheService.Set(refreshKey, "1", refreshCacheDuration);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error
                            }
                            finally
                            {
                                _refreshFlags[cacheKey] = 0;
                            }
                        });
                    }

                    return cachedData;
                }

                // Critical section — ensure only one thread populates the cache
                lock (lockObj)
                {
                    if (_inmemoryCacheService.KeyExists(cacheKey))
                    {
                        return _inmemoryCacheService.Get<T>(cacheKey);
                    }

                    T result = Task.Run(codeBlock).Result;
                    //if (IsDataAvailable(result))
                    //{
                    _inmemoryCacheService.Set(cacheKey, result, cacheDuration);
                    _inmemoryCacheService.Set(refreshKey, "1", refreshCacheDuration);
                    _refreshFlags[cacheKey] = 0;
                    //}
                    //else
                    //{
                    //}

                    return result;
                }
            }
            return await Task.Run(codeBlock);

        }

        //public async Task<T> GetDataAsync<T>(Func<Task<T>> codeBlock, string cacheKey, IDictionary<string, object> paramWithValues, int refreshCacheDuration = 30, string cacheType = "inmemory", int cacheDuration = 180)
        //{
        //    if (_useCaching && cacheDuration > 0)
        //    {
        //        var hash = "";
        //        if (paramWithValues.Count > 0)
        //        {
        //            var paramWithValuesString = string.Join(",", paramWithValues.OrderBy(x => x.Key).Select(p => $"{p.Key}={p.Value}"));
        //           hash = Convert.ToBase64String(MD5.HashData(Encoding.UTF8.GetBytes(paramWithValuesString.ToString())));
        //        }
        //        cacheKey = $"{cacheKey}_{hash}_{cacheType}";
        //        string refreshKey = $"{cacheKey}_refresh";

        //        object lockObj = _cacheLocks.GetOrAdd(cacheKey, new object());

        //        var keyExist = false;
        //        if (cacheType == "redis")
        //        { }
        //            //keyExist = await _redisCacheService.KeyExistsAsync(cacheKey);
        //        else
        //            keyExist = _inmemoryCacheService.KeyExists(cacheKey);
        //        if (keyExist)
        //        {
        //            T? cachedData = default(T);
        //            if (cacheType == "redis")
        //            { }
        //                //cachedData = await _redisCacheService.GetAsync<T>(cacheKey);
        //            else
        //                cachedData = _inmemoryCacheService.Get<T>(cacheKey);

        //            var refreshKeyExist = false;
        //            if (cacheType == "redis")
        //            { }
        //                //refreshKeyExist = await _redisCacheService.KeyExistsAsync(refreshKey);
        //            else
        //                refreshKeyExist = _inmemoryCacheService.KeyExists(refreshKey);

        //            if (!refreshKeyExist && _refreshFlags.TryGetValue(cacheKey, out int flag) && flag == 0)
        //            {
        //                _refreshFlags[cacheKey] = 1;
        //                _ = Task.Run(async () =>
        //                {
        //                    T freshData = await codeBlock();
        //                    if (freshData != null)
        //                    {
        //                        if (cacheType == "redis")
        //                        {
        //                            //await _redisCacheService.SetAsync(cacheKey, freshData, cacheDuration);
        //                            //await _redisCacheService.SetAsync(refreshKey, "1", refreshCacheDuration);
        //                        }
        //                        else
        //                        {
        //                            _inmemoryCacheService.Set(cacheKey, freshData, cacheDuration);
        //                            _inmemoryCacheService.Set(refreshKey, "1", refreshCacheDuration);
        //                        }
        //                    }
        //                });
        //            }
        //            return cachedData;
        //        }

        //        lock (lockObj)
        //        {
        //            bool keyExistLocked = false;
        //            if (cacheType == "redis")
        //            {
        //                //keyExistLocked = _redisCacheService.KeyExistsAsync(cacheKey).GetAwaiter().GetResult();
        //            }
        //            else
        //            {
        //                keyExistLocked = _inmemoryCacheService.KeyExists(cacheKey);
        //            }
        //            if (keyExistLocked)
        //            {
        //                if (cacheType == "redis")
        //                { }
        //                    //return _redisCacheService.GetAsync<T>(cacheKey).GetAwaiter().GetResult();
        //                else
        //                    return _inmemoryCacheService.Get<T>(cacheKey);
        //            }

        //            var result = codeBlock().GetAwaiter().GetResult();
        //            if (result == null)
        //            {
        //                return result;
        //            }

        //            if (cacheType == "redis")
        //            {
        //                //_redisCacheService.SetAsync(cacheKey, result, cacheDuration).GetAwaiter().GetResult();
        //                //_redisCacheService.SetAsync(refreshKey, "1", refreshCacheDuration).GetAwaiter().GetResult();
        //            }
        //            else
        //            {
        //                _inmemoryCacheService.Set(cacheKey, result, cacheDuration);
        //                _inmemoryCacheService.Set(refreshKey, "1", refreshCacheDuration);
        //            }
        //            _refreshFlags[cacheKey] = 0;
        //            return result;
        //        }

        //    }
        //    else
        //    {
        //        return await Task.Run(() =>
        //        {
        //            return codeBlock();
        //        });
        //    }
        //}

    }
}
