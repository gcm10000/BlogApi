using BlogApi.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace BlogApi.Application.Infrastructure.Identity.Services;
public class HybridCache : IHybridCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly string _storagePath;

    public HybridCache(IMemoryCache memoryCache, string storagePath = "cache_store")
    {
        _memoryCache = memoryCache;
        _storagePath = storagePath;

        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);
    }

    public T Get<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T value))
        {
            return value;
        }

        // Fallback para disco
        var filePath = GetFilePath(key);
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var wrapper = JsonSerializer.Deserialize<CacheWrapper<T>>(json);

            if (wrapper?.Expiration > DateTime.UtcNow)
            {
                _memoryCache.Set(key, wrapper.Value, wrapper.Expiration - DateTime.UtcNow);
                return wrapper.Value;
            }

            File.Delete(filePath); // Limpa se expirado
        }

        return default!;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _memoryCache.Set(key, value, expiration);

        var wrapper = new CacheWrapper<T>
        {
            Value = value,
            Expiration = DateTime.UtcNow.Add(expiration)
        };

        var json = JsonSerializer.Serialize(wrapper);
        File.WriteAllText(GetFilePath(key), json);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
        var filePath = GetFilePath(key);
        if (File.Exists(filePath)) File.Delete(filePath);
    }

    private string GetFilePath(string key)
    {
        var safeKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(key));
        return Path.Combine(_storagePath, $"{safeKey}.json");
    }

    private class CacheWrapper<T>
    {
        public T Value { get; set; }
        public DateTime Expiration { get; set; }
    }
}
