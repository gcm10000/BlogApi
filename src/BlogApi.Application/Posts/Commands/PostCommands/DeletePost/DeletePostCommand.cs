using MediatR;

namespace BlogApi.Application.Posts.Commands.PostCommands.DeletePost;

public class DeletePostCommand : IRequest<bool>
{
    public int Id { get; set; }
}
