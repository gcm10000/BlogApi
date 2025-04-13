using BlogApi.Application.Categories.Dto;
using MediatR;

namespace BlogApi.Application.Posts.Queries.GetCategoriesQuery;

// Query
public class GetCategoriesQuery : IRequest<List<CategoryDto>>
{
}
