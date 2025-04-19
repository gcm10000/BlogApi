namespace BlogApi.Application.Tenancies.Dtos;

public class TenancyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsMainTenancy { get; set; }
    public required string MainAdministratorEmail { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
