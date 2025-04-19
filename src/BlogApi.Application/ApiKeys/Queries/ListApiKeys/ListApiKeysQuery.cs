using BlogApi.Application.ApiKeys.Dto;
using MediatR;

namespace BlogApi.Application.ApiKeys.Queries.ListApiKeys;

public record ListApiKeysQuery(string? Name) : IRequest<List<ApiKeyDto>>;

