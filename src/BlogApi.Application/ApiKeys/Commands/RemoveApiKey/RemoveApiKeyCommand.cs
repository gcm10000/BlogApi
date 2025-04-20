using MediatR;

namespace BlogApi.Application.ApiKeys.Commands.RemoveApiKey;
public record RemoveApiKeyCommand(int Id) : IRequest<bool>;
