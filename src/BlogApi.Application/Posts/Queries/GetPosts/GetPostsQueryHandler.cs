using BlogApi.Application.Common;
using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Posts.Queries.GetPosts;

public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, PagedResponse<List<PostDto>>>
{
    private readonly BlogDbContext _db;

    public GetPostsQueryHandler(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResponse<List<PostDto>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Posts
            .Include(x => x.Author)
            .Include(x => x.Tenancy)
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == request.TenancyId)
            .Include(p => p.PostCategories)
            .ThenInclude(pc => pc.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(p => p.Status == request.Status);

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(p => p.Title.Contains(request.Search));

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(p => p.PostCategories.Any(pc => pc.Category.Name == request.Category));

        var total = await query.CountAsync(cancellationToken);
        var posts = await query
            .Where(x => x.TenancyId == request.TenancyId)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Author = p.Author.Name,
                Title = p.Title,
                Excerpt = p.Excerpt,
                Slug = p.Slug,
                Image = p.Image,
                AuthorId = p.AuthorId,
                Status = p.Status,
                Date = p.Date,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Categories = p.PostCategories.Select(pc => pc.Category.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<List<PostDto>>(posts, total, request.Page, request.Limit);
    }
}
