using BlogApi.Application.Categories.Dto;
using BlogApi.Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Posts.Queries.GetCategoriesQuery;

// Query Handler
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly BlogDbContext _context;

    public GetCategoriesQueryHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .Select(c => new CategoryDto { Name = c.Name })
            .ToListAsync(cancellationToken);

        return categories;
    }
}
