using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Domain.Entities;
using BlogApi.Infrastructure.Identity.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Dashboard.Queries.GetDashboardRecent;

public class GetDashboardRecentQueryHandler : IRequestHandler<GetDashboardRecentQuery, DashboardRecentDto>
{
    private readonly BlogDbContext _context;
    private readonly IdentityDbContext _identityDbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetDashboardRecentQueryHandler(
        BlogDbContext context, 
        IdentityDbContext identityDbContext, 
        ICurrentUserService currentUserService)
    {
        _context = context;
        _identityDbContext = identityDbContext;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardRecentDto> Handle(GetDashboardRecentQuery request, CancellationToken cancellationToken)
    {
        var tenancyId = _currentUserService.GetCurrentTenancy();

        var posts = await _context.Posts
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == tenancyId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new RecentPostDto
            {
                Id = p.Id,
                Title = p.Title,
                Status = p.Status,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var users = await _identityDbContext.Users
            .Where(x => x.TenancyDomainId == tenancyId)
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .Select(u => new RecentUserDto
            {
                Id = u.AuthorId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync(cancellationToken);

        return new DashboardRecentDto
        {
            Posts = posts,
            Users = users
        };
    }
}