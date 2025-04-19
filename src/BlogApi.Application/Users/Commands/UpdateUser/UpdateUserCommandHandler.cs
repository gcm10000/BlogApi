using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Infrastructure.Identity.Models;
using BlogApi.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        BlogDbContext context,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.AuthorId == request.Id, cancellationToken);

            if (user == null)
                return null;

            // Atualiza dados do Identity
            user.Email = request.Email;
            user.Name = request.Name;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                // Se falhar, cancela transação
                await transaction.RollbackAsync(cancellationToken);
                return null;
            }

            var tenancyId = _currentUserService.GetCurrentTenancyDomainId();

            // Atualiza dados do Author
            var author = await _context.Authors
                .Include(x => x.Tenancy)
                .Where(x => x.Tenancy.DeletedAt == null)
                .Where(x => x.TenancyId == tenancyId)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (author != null)
            {
                author.Name = request.Name;
                author.UpdatedAt = DateTime.UtcNow;

                _context.Authors.Update(author);
                await _context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return new UserDto
            {
                Id = user.AuthorId,
                Name = user.Name,
                IsMainTenancy = user.IsMainTenancy,
                Email = user.Email,
                TenancyDomainId = user.TenancyDomainId,
                TenancyDomainName = user.TenancyDomainName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
