using BlogApi.Application.Infrastructure.Identity.Dtos;

namespace BlogApi.Application.Interfaces;
public interface IApiKeyServices
{
    Task<ApiKeyValidationResultDto> GetApiKeyAsync(string apiKeyValue);
}
