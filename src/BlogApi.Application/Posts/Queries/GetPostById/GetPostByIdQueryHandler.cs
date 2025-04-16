using MediatR;
using Microsoft.EntityFrameworkCore;
using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;

namespace BlogApi.Application.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto?>
{
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetPostByIdQueryHandler(BlogDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(x => x.PostCategories)
                .ThenInclude(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tenancy)
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == request.TenancyId)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (post == null) return null;

        return new PostDto
        {
            Id = post.Id,
            Slug = post.Slug,
            Title = post.Title,
            AuthorId = post.AuthorId,
            Content = post.Content,
            Categories = post.PostCategories.Select(x => x.Category.Name).ToList(),
            Author = post.Author.Name,
            Excerpt = post.Excerpt,
            Image = post.Image,
            Status = post.Status,
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
