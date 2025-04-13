using BlogApi.Application.Categories.Dto;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Domain.Entities;
using MediatR;

namespace BlogApi.Application.Posts.Commands.AddCategory;

// Command Handler
public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, CategoryDto>
{
    private readonly BlogDbContext _context;

    public AddCategoryCommandHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryDto { Name = category.Name };
    }
}
