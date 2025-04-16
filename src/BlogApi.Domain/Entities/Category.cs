namespace BlogApi.Domain.Entities;

public class Category : Entity
{
    public string Name { get; set; } = "";
    public List<PostCategory> PostCategories { get; set; } = new();

    public Tenancy Tenancy { get; set; } = null!;
    public int TenancyId { get; set; }
}