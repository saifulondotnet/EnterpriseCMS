using System.Text.Json;
using EnterpriseCMS.Core.Interfaces;
using StackExchange.Redis;

namespace EnterpriseCMS.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis) => _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(60));
    }

    public Task RemoveAsync(string key, CancellationToken ct = default) => _db.KeyDeleteAsync(key);

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
        var keys = server.Keys(pattern: $"{prefix}*").ToArray();
        if (keys.Length > 0) await _db.KeyDeleteAsync(keys);
    }
}
