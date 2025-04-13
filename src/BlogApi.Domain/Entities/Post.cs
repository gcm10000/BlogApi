namespace BlogApi.Domain.Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string Image { get; set; } = "";
    public int AuthorId { get; set; }
    public string Status { get; set; } = "draft";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<PostCategory> PostCategories { get; set; } = new();
}