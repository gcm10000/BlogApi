using MediatR;

namespace BlogApi.Application.Posts.Commands.CategoryCommands.DeleteCategory;

// Command
public class DeleteCategoryCommand : IRequest<bool>
{
    public string Name { get; set; }
}
