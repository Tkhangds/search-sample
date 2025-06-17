using System.IO.Compression;
using System.Text;
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

    private byte[] Compress(string data)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            using (var output = new MemoryStream())
            using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
            {
                gzip.Write(bytes, 0, bytes.Length);
                gzip.Close();

                return output.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception("Error while compressing.", ex);
        }
        
    }

    private string Decompress(byte[] compressed)
    {
        try
        {
            using (var input = new MemoryStream(compressed))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzip, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception("Error while decompressing.", ex);
        }
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var compressed = await _cache.GetAsync(key);
        if (compressed == null) return default;
        
        var data = Decompress(compressed);
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options)
    {
        
        options ??= new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        
        var data = JsonSerializer.Serialize(value);
        var compressed = Compress(data);

        await _db.SetAddAsync(KeysSetKey, key);
        
        await _cache.SetAsync(key, compressed, options);
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