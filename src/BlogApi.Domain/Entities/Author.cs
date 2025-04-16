namespace BlogApi.Domain.Entities;

public class Author : Entity
{
    public string Name { get; set; }

    public string Bio { get; set; }

    public List<Post> Posts { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Tenancy Tenancy { get; set; } = null!;
    public int TenancyId { get; set; }
}