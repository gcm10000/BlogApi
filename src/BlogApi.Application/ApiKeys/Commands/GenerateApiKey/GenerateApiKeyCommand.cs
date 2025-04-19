using BlogApi.Application.ApiKeys.Dto;
using MediatR;

namespace BlogApi.Application.ApiKeys.Commands.GenerateApiKey;
public record GenerateApiKeyCommand(
    string Name,
    List<string> Scopes
) : IRequest<GenerateApiKeyResult>; // Retorna a API Key gerada
