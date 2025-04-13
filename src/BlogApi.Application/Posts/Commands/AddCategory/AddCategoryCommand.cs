using BlogApi.Application.Categories.Dto;
using MediatR;

namespace BlogApi.Application.Posts.Commands.AddCategory;

// Command
public class AddCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; }
}
