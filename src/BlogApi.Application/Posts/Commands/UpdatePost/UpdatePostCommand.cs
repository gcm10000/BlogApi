using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Posts.Commands.UpdatePost;

public class UpdatePostCommand : IRequest<PostDto>
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string Image { get; set; } = "";
    public List<string> Categories { get; set; } = new();
    public string Status { get; set; } = "draft";
}
