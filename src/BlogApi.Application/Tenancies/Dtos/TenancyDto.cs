namespace BlogApi.Application.Tenancies.Dtos;

public class TenancyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
