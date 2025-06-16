using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Eme_Search.Common.Cache;

public class RedisCacheService: ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _db;
    private const string KeysSetKey = "cache_keys";
    public RedisCacheService(IDistributedCache cache, ConnectionMultiplexer connection )
    {
        _cache = cache;
        _connection = connection;
        _db = _connection.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data)) return default;
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options)
    {
        
        options ??= new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        
        var data = JsonSerializer.Serialize(value);
        
        await _db.SetAddAsync(KeysSetKey, key);
        
        await _cache.SetStringAsync(key, data, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
    
    public async Task ClearAllAsync()
    {
        var keys = await _db.SetMembersAsync(KeysSetKey);
        foreach (var key in keys)
        {
            await _cache.RemoveAsync(key);
        }
        await _db.KeyDeleteAsync(KeysSetKey);
    }
}