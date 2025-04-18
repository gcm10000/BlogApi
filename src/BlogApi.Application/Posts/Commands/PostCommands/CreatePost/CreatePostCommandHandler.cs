using MediatR;
using BlogApi.Domain.Entities;
using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using BlogApi.Application.Interfaces;
using BlogApi.Application.Helpers;
using BlogApi.Application.Infrastructure;

namespace BlogApi.Application.Posts.Commands.PostCommands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreatePostCommandHandler(BlogDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }


    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var tenancyId = _currentUserService.GetCurrentTenancy();
        var authorId = _currentUserService.GetCurrentAuthorId();

        var baseSlug = SlugHelper.GenerateSlug(request.Title);
        var uniqueSlug = await GenerateUniqueSlugAsync(baseSlug, tenancyId, cancellationToken);

        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadPath);

        var imagePath = await ImageUploader.SaveImageAsync(request.ImageFile, request.ImageUrl, uploadPath);

        var post = new Post
        {
            Title = request.Title,
            Slug = uniqueSlug,
            Content = request.Content,
            Excerpt = request.Excerpt,
            Image = imagePath ?? "",
            AuthorId = authorId,
            Status = request.Status,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TenancyId = tenancyId
        };

        var existingCategories = await _context.Categories
            .Include(x => x.Tenancy)
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == tenancyId)
            .Where(c => request.Categories.Contains(c.Name))
            .ToListAsync(cancellationToken);

        foreach (var catName in request.Categories)
        {
            var category = existingCategories.FirstOrDefault(c => c.Name == catName);
            if (category == null)
            {
                category = new Category { Name = catName, TenancyId = tenancyId };
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
            Slug = post.Slug,
            Title = post.Title,
            Excerpt = post.Excerpt,
            Content = post.Content,
            AuthorId = post.AuthorId,
            Categories = request.Categories,
            Image = post.Image,
            Status = post.Status,
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    private async Task<string> GenerateUniqueSlugAsync(string baseSlug, int tenancyId, CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        int counter = 1;

        while (await _context.Posts.AnyAsync(
            p => p.TenancyId == tenancyId && p.Slug == slug,
            cancellationToken))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

}
