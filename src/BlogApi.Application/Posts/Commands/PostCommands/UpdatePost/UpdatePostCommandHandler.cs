using BlogApi.Application.DTOs;
using BlogApi.Application.Exceptions;
using BlogApi.Application.Helpers;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using BlogApi.Application.Infrastructure;

namespace BlogApi.Application.Posts.Commands.PostCommands.UpdatePost;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostDto>
{
    private readonly BlogDbContext _db;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePostCommandHandler(BlogDbContext db, ICurrentUserService currentUserService)
    {
        _db = db;
        _currentUserService = currentUserService;
    }

    public async Task<PostDto> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var tenancyId = _currentUserService.GetCurrentTenancy();

        var post = await _db.Posts
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Where(p => p.TenancyId == tenancyId && p.Tenancy.DeletedAt == null)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (post == null)
            throw new BusinessRuleException("Post não encontrado.");

        post.Title = request.Title;
        post.Slug = SlugHelper.GenerateSlug(request.Title);
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.Status = request.Status;
        post.UpdatedAt = DateTime.UtcNow;

        //// Verifica se uma imagem foi enviada (ImageFile tem prioridade sobre ImageUrl)
        //if (request.ImageFile != null)
        //{
        //    // Lógica para salvar o arquivo de imagem e atualizar a URL
        //    // Exemplo fictício, adaptação necessária para salvar o arquivo na sua estrutura
        //    var fileName = Path.GetFileName(request.ImageFile.FileName);
        //    var filePath = Path.Combine("wwwroot", "images", fileName); // Ajuste conforme seu diretório de armazenamento
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await request.ImageFile.CopyToAsync(stream);
        //    }
        //    post.Image = "/images/" + fileName; // Caminho da imagem
        //}
        //else if (!string.IsNullOrEmpty(request.ImageUrl))
        //{
        //    post.Image = request.ImageUrl; // Utiliza a URL da imagem enviada
        //}

        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadPath);
        var imagePath = await ImageUploader.SaveImageAsync(request.ImageFile, request.ImageUrl, uploadPath);

        post.Image = imagePath ?? "";
        var existingCategories = await _db.Categories
            .Where(c => request.Categories.Contains(c.Name) && c.TenancyId == tenancyId)
            .ToListAsync(cancellationToken);

        post.PostCategories.Clear();

        foreach (var catName in request.Categories)
        {
            var category = existingCategories.FirstOrDefault(c => c.Name == catName);
            if (category == null)
            {
                category = new Category { Name = catName, TenancyId = tenancyId };
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
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            AuthorId = post.AuthorId,
            Categories = post.PostCategories.Select(pc => pc.Category.Name).ToList()
        };
    }
}
