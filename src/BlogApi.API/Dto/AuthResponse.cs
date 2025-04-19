namespace BlogApi.API.Dto;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Name { get; set; }
    public required string TenancyId { get; set; }
    public required string TenancyName { get; set; }
    public required bool IsMainTenancy { get; set; }
    public string Username { get; set; }
    public bool PasswordChangeRequired { get; set; }
    public string Role { get; set; }
}
