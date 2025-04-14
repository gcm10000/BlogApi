using BlogApi.Application.Infrastructure.Data;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly BlogDbContext _context;

    public DeleteUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        BlogDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.AuthorId == request.Id, cancellationToken);

            if (user == null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            var identityResult = await _userManager.DeleteAsync(user);

            if (!identityResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
