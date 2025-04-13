using MediatR;
using Microsoft.EntityFrameworkCore;
using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;

namespace BlogApi.Application.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto?>
{
    private readonly BlogDbContext _context;

    public GetPostByIdQueryHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<PostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (post == null) return null;

        return new PostDto
        {
            Id = post.Id,
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
