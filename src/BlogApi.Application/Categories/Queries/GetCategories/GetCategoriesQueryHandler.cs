using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Categories.Queries.GetCategories;

// Query Handler
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly BlogDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCategoriesQueryHandler(BlogDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var currentTenancy = _currentUserService.GetCurrentTenancyDomainId();

        var categories = await _context.Categories
            .Where(x => x.TenancyId == currentTenancy)
            .Select(c => new CategoryDto { Name = c.Name })
            .ToListAsync(cancellationToken);

        return categories;
    }
}
