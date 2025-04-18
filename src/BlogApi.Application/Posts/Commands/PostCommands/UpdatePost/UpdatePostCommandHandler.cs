using BlogApi.Application.DTOs;
using BlogApi.Application.Exceptions;
using BlogApi.Application.Helpers;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BlogApi.Application.Infrastructure;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace BlogApi.Application.Posts.Commands.PostCommands.UpdatePost;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostDto>
{
    private readonly BlogDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdatePostCommandHandler> _logger;

    public UpdatePostCommandHandler(BlogDbContext db, ICurrentUserService currentUserService, ILogger<UpdatePostCommandHandler> logger)
    {
        _db = db;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PostDto> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started handling UpdatePostCommand for Post ID: {PostId}", request.Id);

        var tenancyId = _currentUserService.GetCurrentTenancy();
        _logger.LogInformation("Retrieved TenancyId: {TenancyId}", tenancyId);

        var post = await _db.Posts
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Where(p => p.TenancyId == tenancyId && p.Tenancy.DeletedAt == null)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (post == null)
        {
            _logger.LogError("Post with ID: {PostId} not found.", request.Id);
            throw new BusinessRuleException("Post não encontrado.");
        }

        _logger.LogInformation("Post found. Updating Post with ID: {PostId}", post.Id);

        post.Title = request.Title;
        post.Slug = SlugHelper.GenerateSlug(request.Title);
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.Status = request.Status;
        post.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Post updated with new Title: {Title}, Slug: {Slug}", post.Title, post.Slug);

        //string uploadPath = string.Empty;
        //    uploadPath = Directory.GetCurrentDirectory();

        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //{
        //    _logger.LogInformation("Running on Windows, using upload path: {UploadPath}", uploadPath);
        //}
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //{
        //    uploadPath = "/app";
        //    _logger.LogInformation("Running on Linux, using upload path: {UploadPath}", uploadPath);
        //}

        try
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadPath);
            _logger.LogInformation("Created directory at: {UploadPath}", uploadPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create directory at: {UploadPath}", uploadPath);
            throw;
        }

        var imagePath = await ImageUploader.SaveImageAsync(request.ImageFile, request.ImageUrl, uploadPath);

        if (imagePath != null)
        {
            post.Image = imagePath;
            _logger.LogInformation("Image saved at: {ImagePath}", imagePath);
        }

        var existingCategories = await _db.Categories
            .Where(c => request.Categories.Contains(c.Name) && c.TenancyId == tenancyId)
            .ToListAsync(cancellationToken);

        post.PostCategories.Clear();
        _logger.LogInformation("Cleared existing post categories.");

        foreach (var catName in request.Categories)
        {
            var category = existingCategories.FirstOrDefault(c => c.Name == catName);
            if (category == null)
            {
                category = new Category { Name = catName, TenancyId = tenancyId };
                _db.Categories.Add(category);
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
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Post with ID: {PostId} successfully updated and saved to database.", post.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save updated post with ID: {PostId} to the database.", post.Id);
            throw;
        }

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Excerpt = post.Excerpt,
            Image = post.Image,
            Status = post.Status,
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            AuthorId = post.AuthorId,
            Categories = post.PostCategories.Select(pc => pc.Category.Name).ToList()
        };
    }
}
