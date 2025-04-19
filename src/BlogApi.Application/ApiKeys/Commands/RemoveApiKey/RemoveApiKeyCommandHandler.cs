using BlogApi.Application.Exceptions;
using BlogApi.Application.Infrastructure.Identity.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.ApiKeys.Commands.RevokeApiKey;

public class RemoveApiKeyCommandHandler : IRequestHandler<RemoveApiKeyCommand, bool>
{
    private readonly IdentityDbContext _context;

    public RemoveApiKeyCommandHandler(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveApiKeyCommand request, CancellationToken cancellationToken)
    {
        var apiKey = await _context.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (apiKey is null)
            return false;

        if (apiKey.IsProtected)
            throw new BusinessRuleException("Esta chave de acesso é protegida e não pode ser modificada ou removida.");

        _context.ApiKeys.Remove(apiKey);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
