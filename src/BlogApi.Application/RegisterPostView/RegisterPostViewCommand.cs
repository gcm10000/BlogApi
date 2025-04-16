using MediatR;

namespace BlogApi.Application.RegisterPostView;

public class RegisterPostViewCommand : INotification
{
    public int PostId { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }

    public RegisterPostViewCommand(int postId, string? ipAddress, string? userAgent)
    {
        PostId = postId;
        IPAddress = ipAddress;
        UserAgent = userAgent;
    }
}
