using BlogApi.Application.Infrastructure.Identity.Dtos;

namespace BlogApi.Application.Interfaces;
public interface IApiKeyService
{
    Task<ApiKeyValidationResultDto> GetApiKeyAsync(string apiKeyValue);
}
