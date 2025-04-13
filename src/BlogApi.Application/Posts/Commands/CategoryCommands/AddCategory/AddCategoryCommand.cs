using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Posts.Commands.CategoryCommands.AddCategory;

// Command
public class AddCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; }
}
