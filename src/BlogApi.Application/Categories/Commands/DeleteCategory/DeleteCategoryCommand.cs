using MediatR;

namespace BlogApi.Application.Categories.Commands.DeleteCategory;

// Command
public class DeleteCategoryCommand : IRequest<bool>
{
    public string Name { get; set; }
}
