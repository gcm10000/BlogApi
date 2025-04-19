using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Domain.Entities;
using MediatR;

namespace BlogApi.Application.Categories.Commands.AddCategory;

// Command Handler
public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, CategoryDto>
{
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddCategoryCommandHandler(BlogDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CategoryDto> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var tenancyId = _currentUserService.GetCurrentTenancyDomainId();
        var category = new Category
        {
            Name = request.Name,
            TenancyId = tenancyId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryDto { Name = category.Name };
    }
}
