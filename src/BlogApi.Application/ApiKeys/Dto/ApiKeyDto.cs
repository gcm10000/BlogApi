using BlogApi.Application.Infrastructure.Identity.Models;

namespace BlogApi.Application.ApiKeys.Dto;
public record ApiKeyDto(
    int Id,
    string Name,
    string Key,
    DateTime CreatedAt,
    bool IsActive,
    List<ApiScope> Scopes
);