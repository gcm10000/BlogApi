using MediatR;
using BlogApi.Application.DTOs;

namespace BlogApi.Application.Posts.Commands.CreatePost;

public class CreatePostCommand : IRequest<PostDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public List<string> Categories { get; set; } = new();
    public string Status { get; set; } = "draft";
}
