namespace BlogApi.Domain.Entities;

public class PostView : Entity
{
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime ViewedAt { get; set; }
}