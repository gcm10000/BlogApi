using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Common;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponse<List<UserDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly BlogDbContext _context;

    public GetUsersQueryHandler(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        BlogDbContext context)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _context = context;
    }

    public async Task<PagedResponse<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var currentTenancy = _currentUserService.GetCurrentTenancy();

        // Query de usuários da tenancy atual
        var query = _userManager.Users
            .Where(x => x.TenancyDomainId == currentTenancy);

        // Filtro de busca
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(u =>
                u.UserName.Contains(request.Search) ||
                u.Email.Contains(request.Search));
        }

        var total = await query.CountAsync(cancellationToken);

        // Buscar autores paginados
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        var authorIds = users.Select(u => u.AuthorId).ToList();

        // Buscar quantidade de posts por autor
        var postCounts = await _context.Posts
            .Where(p => authorIds.Contains(p.AuthorId))
            .GroupBy(p => p.AuthorId)
            .Select(g => new { AuthorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.AuthorId, g => g.Count, cancellationToken);

        // Montar os DTOs com contagem de posts
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.AuthorId,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            Posts = postCounts.TryGetValue(u.AuthorId, out var count) ? count : 0,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();

        return new PagedResponse<List<UserDto>>(userDtos, total, request.Page, request.Limit);
    }
}
