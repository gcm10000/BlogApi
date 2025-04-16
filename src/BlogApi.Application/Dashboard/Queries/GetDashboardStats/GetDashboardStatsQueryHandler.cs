using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDashboardStatsQueryHandler(BlogDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var tenancyId = _currentUserService.GetCurrentTenancy();
        var publishedPosts = await _context.Posts
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == tenancyId)
            .CountAsync(p => p.Status == "published", cancellationToken);

        var draftPosts = await _context.Posts
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == tenancyId)
            .CountAsync(p => p.Status == "draft", cancellationToken);

        var totalUsers = await _context.Authors
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == tenancyId)
            .CountAsync(cancellationToken);

        var totalViews = await _context.PostViews
            .Include(x => x.Post)
                .ThenInclude(x => x.Tenancy)
            .Where(x => x.Post.Tenancy.DeletedAt == null)
            .Where(x => x.Post.TenancyId == tenancyId)
            .CountAsync(cancellationToken);

        return new DashboardStatsDto
        {
            PublishedPosts = publishedPosts,
            DraftPosts = draftPosts,
            TotalUsers = totalUsers,
            TotalViews = totalViews
        };
    }
}