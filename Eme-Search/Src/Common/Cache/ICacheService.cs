using Microsoft.Extensions.Caching.Distributed;

namespace Eme_Search.Common.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options);
    Task RemoveAsync(string key);
}