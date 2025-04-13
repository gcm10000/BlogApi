using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Posts.Commands.UpdatePostStatus;

public class UpdatePostStatusCommand : IRequest<PostDto>
{
    public int Id { get; set; }
    public string Status { get; set; }
}
