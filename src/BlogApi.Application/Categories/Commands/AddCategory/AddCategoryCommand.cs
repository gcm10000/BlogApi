using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Categories.Commands.AddCategory;

public class AddCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; }
}
