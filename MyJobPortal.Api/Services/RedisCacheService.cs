using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        _cache = cache;
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data))
        {
            return default;
        }
        _logger.LogDebug("Cache hit for the key {CacheKey}", key);
        return JsonSerializer.Deserialize<T>(data);
    }
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
        };
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options);
        _logger.LogDebug("Cache set for key {CacheKey}", key);
    }
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        _logger.LogDebug("Cache removed for key {CacheKey}", key);
    }
}