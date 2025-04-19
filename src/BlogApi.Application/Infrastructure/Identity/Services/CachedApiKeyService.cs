using BlogApi.Application.Infrastructure.Identity.Dtos;
using BlogApi.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BlogApi.Application.Infrastructure.Identity.Services;
public class CachedApiKeyService : IApiKeyService
{
    private readonly IApiKeyService _inner;
    private readonly IHybridCache _cache;

    public CachedApiKeyService(IApiKeyService inner, IHybridCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<ApiKeyValidationResultDto> GetApiKeyAsync(string apiKeyValue)
    {
        var key = $"apikey_{apiKeyValue}";
        var result = _cache.Get<ApiKeyValidationResultDto>(key);

        if (result != null)
            return result;

        result = await _inner.GetApiKeyAsync(apiKeyValue);

        _cache.Set(key, result, TimeSpan.FromHours(24));
        return result;
    }
}