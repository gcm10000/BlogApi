namespace BlogApi.API.Dto;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; }
    public bool PasswordChangeRequired { get; set; }
    public string Role { get; set; }
}
