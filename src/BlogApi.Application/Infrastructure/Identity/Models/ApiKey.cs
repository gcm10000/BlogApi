namespace BlogApi.Application.Infrastructure.Identity.Models;

public class ApiKey
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Key { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public required bool IsProtected { get; set; } = false;

    public int TenancyDomainId { get; set; }
    public List<ApiKeyScope> ApiKeyScopes { get; set; } = new();
}