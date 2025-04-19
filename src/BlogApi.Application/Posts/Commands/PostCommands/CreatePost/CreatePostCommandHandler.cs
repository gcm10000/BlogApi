using MediatR;
using BlogApi.Domain.Entities;
using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using BlogApi.Application.Interfaces;
using BlogApi.Application.Helpers;
using BlogApi.Application.Infrastructure;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace BlogApi.Application.Posts.Commands.PostCommands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreatePostCommandHandler> _logger;

    public CreatePostCommandHandler(BlogDbContext context, ICurrentUserService currentUserService, ILogger<CreatePostCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting to handle CreatePostCommand...");

        var tenancyId = _currentUserService.GetCurrentTenancyDomainId();
        var authorId = _currentUserService.GetCurrentAuthorId();

        _logger.LogInformation("Fetched tenancyId: {TenancyId}, authorId: {AuthorId}", tenancyId, authorId);

        var baseSlug = SlugHelper.GenerateSlug(request.Title);
        var uniqueSlug = await GenerateUniqueSlugAsync(baseSlug, tenancyId, cancellationToken);

        _logger.LogInformation("Generated unique slug: {Slug}", uniqueSlug);

        string uploadPath = string.Empty;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            uploadPath = Directory.GetCurrentDirectory();
            _logger.LogInformation("Running on Windows, upload path: {UploadPath}", uploadPath);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            uploadPath = "/app";
            _logger.LogInformation("Running on Linux, upload path: {UploadPath}", uploadPath);
        }

        try
        {
            Directory.CreateDirectory(uploadPath);
            _logger.LogInformation("Created upload directory at: {UploadPath}", uploadPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create upload directory at: {UploadPath}", uploadPath);
            throw;
        }

        var imagePath = await ImageUploader.SaveImageAsync(request.ImageFile, request.ImageUrl, uploadPath);
        _logger.LogInformation("Image saved at: {ImagePath}", imagePath ?? "No image");

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

        _logger.LogInformation("Created new Post entity with title: {Title}", post.Title);

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
                _logger.LogInformation("Added new category: {CategoryName}", catName);
            }

            post.PostCategories.Add(new PostCategory
            {
                Post = post,
                Category = category
            });
        }

        try
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Post saved to database with ID: {PostId}", post.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save post to database.");
            throw;
        }

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
        _logger.LogInformation("Generating unique slug for base slug: {BaseSlug}", baseSlug);

        var slug = baseSlug;
        int counter = 1;

        while (await _context.Posts.AnyAsync(
            p => p.TenancyId == tenancyId && p.Slug == slug,
            cancellationToken))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
            _logger.LogInformation("Slug {Slug} already exists, trying next with counter: {Counter}", slug, counter);
        }

        _logger.LogInformation("Generated unique slug: {Slug}", slug);
        return slug;
    }
}
