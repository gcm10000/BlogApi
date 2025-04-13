using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Categories.Commands.AddCategory;

// Command
public class AddCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; }
}
