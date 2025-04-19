using System.Text.Json;

namespace BlogApi.Application.Infrastructure;

public class FileCacheService<T>
{
    private readonly string _cacheDirectory;
    private readonly TimeSpan _expiration;

    public FileCacheService(string cacheDirectory, TimeSpan expiration)
    {
        _cacheDirectory = cacheDirectory;
        _expiration = expiration;

        if (!Directory.Exists(_cacheDirectory))
            Directory.CreateDirectory(_cacheDirectory);
    }

    private string GetCachePath(string key)
    {
        var safeKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(key));
        return Path.Combine(_cacheDirectory, $"{safeKey}.json");
    }

    public void Set(string key, T value)
    {
        var entry = new CacheEntry<T>
        {
            Value = value,
            Expiration = DateTime.UtcNow.Add(_expiration)
        };

        var json = JsonSerializer.Serialize(entry);
        File.WriteAllText(GetCachePath(key), json);
    }

    public T? Get(string key)
    {
        var path = GetCachePath(key);
        if (!File.Exists(path)) return default;

        try
        {
            var json = File.ReadAllText(path);
            var entry = JsonSerializer.Deserialize<CacheEntry<T>>(json);

            if (entry == null || entry.Expiration < DateTime.UtcNow)
            {
                File.Delete(path); // Expirado
                return default;
            }

            return entry.Value;
        }
        catch
        {
            return default;
        }
    }

    private class CacheEntry<TValue>
    {
        public TValue? Value { get; set; }
        public DateTime Expiration { get; set; }
    }
}
