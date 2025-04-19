using BlogApi.Application.Infrastructure.Identity.Dtos;
using BlogApi.Application.Interfaces;

namespace BlogApi.Application.Infrastructure.Identity.Services;

public class DiskCachedApiKeyService : IApiKeyService
{
    private readonly IApiKeyService _inner;
    private readonly FileCacheService<ApiKeyValidationResultDto> _cache;
    private const string CachePrefix = "apikey_";

    public DiskCachedApiKeyService(IApiKeyService inner)
    {
        _inner = inner;
        _cache = new FileCacheService<ApiKeyValidationResultDto>(
            cacheDirectory: Path.Combine(AppContext.BaseDirectory, "cache"),
            expiration: TimeSpan.FromHours(24)
        );
    }

    public async Task<ApiKeyValidationResultDto> GetApiKeyAsync(string apiKeyValue)
    {
        var cacheKey = $"{CachePrefix}{apiKeyValue}";
        var cached = _cache.Get(cacheKey);

        if (cached != null)
            return cached;

        var result = await _inner.GetApiKeyAsync(apiKeyValue);

        if (result.IsValid)
            _cache.Set(cacheKey, result);

        return result;
    }
}
