namespace BlogApi.Application.ApiKeys.Dto;

public record GenerateApiKeyResult(
    string Name,
    string Key,
    int TenancyDomainId,
    List<string> Scopes,
    DateTime CreatedAt
);