using MediatR;
using Microsoft.EntityFrameworkCore;
using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Domain.Entities;
using BlogApi.Application.RegisterPostView;

namespace BlogApi.Application.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQueryHandler : IRequestHandler<GetPostBySlugQuery, PostDto?>
{
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPublisher _publisher;

    public GetPostBySlugQueryHandler(BlogDbContext context, ICurrentUserService currentUserService, IPublisher publisher)
    {
        _context = context;
        _currentUserService = currentUserService;
        _publisher = publisher;
    }

    public async Task<PostDto?> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(x => x.Author)
            .Include(x => x.Tenancy)
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == request.TenancyId)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);

        if (post == null) return null;

        return new PostDto
        {
            Id = post.Id,
            Author = post.Author.Name,
            Slug = post.Slug,
            Content = post.Content,
            Categories = post.PostCategories.Select(x => x.Category.Name).ToList(),
            AuthorId = post.AuthorId,
            Title = post.Title,
            Excerpt = post.Excerpt,
            Image = post.Image,
            Status = post.Status,
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
