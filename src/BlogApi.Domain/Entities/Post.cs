namespace BlogApi.Domain.Entities;

public class Post : Entity
{
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string Image { get; set; } = "";
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public string Status { get; set; } = "draft";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<PostCategory> PostCategories { get; set; } = new();

    public Tenancy Tenancy { get; set; } = null!;
    public int TenancyId { get; set; }
}