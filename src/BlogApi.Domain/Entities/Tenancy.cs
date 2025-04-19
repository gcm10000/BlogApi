namespace BlogApi.Domain.Entities;

public class Tenancy : Entity
{
    public string Name { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public int MainAuthorId { get; set; }
    public bool IsMainTenancy { get; set; }
}