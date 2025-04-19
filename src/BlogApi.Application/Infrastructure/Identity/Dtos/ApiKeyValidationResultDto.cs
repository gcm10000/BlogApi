namespace BlogApi.Application.Infrastructure.Identity.Dtos;
public class ApiKeyValidationResultDto
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string Name { get; set; }
    public int ApiKeyId { get; set; }
    public int TenancyDomainId { get; set; }

    public List<string> Scopes { get; set; } = new();
}
