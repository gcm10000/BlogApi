namespace BlogApi.Application.Infrastructure.Identity.Models;

public class ApiKey
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public int TenancyDomainId { get; set; }
}