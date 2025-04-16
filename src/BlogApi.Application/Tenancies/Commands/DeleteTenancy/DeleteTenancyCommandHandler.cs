using BlogApi.Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Tenancies.Commands.DeleteTenancy;

public class DeleteTenancyCommandHandler : IRequestHandler<DeleteTenancyCommand, bool>
{
    private readonly BlogDbContext _context;

    public DeleteTenancyCommandHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteTenancyCommand request, CancellationToken cancellationToken)
    {
        // Procurar a tenancy a ser excluída pelo Id
        var tenancy = await _context.Tenancies
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tenancy == null)
            return false; // Tenancy não encontrada
        tenancy.DeletedAt = DateTime.UtcNow;
        // Remover a tenancy do contexto
        _context.Tenancies.Update(tenancy);
        await _context.SaveChangesAsync(cancellationToken);

        return true; // Tenancy excluída com sucesso
    }
}