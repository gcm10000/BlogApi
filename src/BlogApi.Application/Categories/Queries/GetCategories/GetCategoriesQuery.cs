using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Categories.Queries.GetCategories;

// Query
public class GetCategoriesQuery : IRequest<List<CategoryDto>>
{
}
