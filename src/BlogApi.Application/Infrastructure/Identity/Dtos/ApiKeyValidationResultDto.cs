namespace BlogApi.Application.Infrastructure.Identity.Dtos;
public class ApiKeyValidationResultDto
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public int ApiKeyId { get; set; }
    public int TenantId { get; set; }
    //public string TenantName { get; set; } = string.Empty;

    public List<string> Scopes { get; set; } = new();
}
