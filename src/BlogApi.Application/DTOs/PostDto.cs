namespace BlogApi.Application.DTOs;

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Excerpt { get; set; }
    public string Image { get; set; }
    public string Status { get; set; }
    public List<string> Categories { get; set; } = new List<string>();
    public int AuthorId { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
