using MediatR;

namespace BlogApi.Application.ApiKeys.Commands.RevokeApiKey;
public record RemoveApiKeyCommand(int Id) : IRequest<bool>;
