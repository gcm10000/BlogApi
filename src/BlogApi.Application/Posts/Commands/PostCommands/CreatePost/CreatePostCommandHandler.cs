using MediatR;
using BlogApi.Domain.Entities;
using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BlogApi.Application.Posts.Commands.PostCommands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly BlogDbContext _context;

    public CreatePostCommandHandler(BlogDbContext context)
    {
        _context = context;
    }


    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            Excerpt = request.Excerpt,
            Image = request.Image,
            AuthorId = request.AuthorId,
            Status = request.Status,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var existingCategories = await _context.Categories
            .Where(c => request.Categories.Contains(c.Name))
            .ToListAsync(cancellationToken);

        foreach (var catName in request.Categories)
        {
            var category = existingCategories.FirstOrDefault(c => c.Name == catName);
            if (category == null)
            {
                category = new Category { Name = catName };
                _context.Categories.Add(category);
            }

            post.PostCategories.Add(new PostCategory
            {
                Post = post,
                Category = category
            });
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

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
