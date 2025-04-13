using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostDto>
{
    private readonly BlogDbContext _db;

    public UpdatePostCommandHandler(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<PostDto> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (post == null)
            throw new Exception("Post not found.");

        post.Title = request.Title;
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.Image = request.Image;
        post.Status = request.Status;
        post.UpdatedAt = DateTime.UtcNow;

        var existingCategories = await _db.Categories
            .Where(c => request.Categories.Contains(c.Name))
            .ToListAsync(cancellationToken);

        // Atualizar categorias
        post.PostCategories.Clear();
        foreach (var catName in request.Categories)
        {
            var category = existingCategories.FirstOrDefault(c => c.Name == catName);
            if (category == null)
            {
                category = new Category { Name = catName };
                _db.Categories.Add(category);
            }
            post.PostCategories.Add(new PostCategory
            {
                Post = post,
                Category = category
            });
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Excerpt = post.Excerpt,
            Image = post.Image,
            Status = post.Status,
            Categories = post.PostCategories.Select(pc => pc.Category.Name).ToList(),
            AuthorId = post.AuthorId,
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
