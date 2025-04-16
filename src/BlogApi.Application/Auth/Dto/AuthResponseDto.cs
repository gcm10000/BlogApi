namespace BlogApi.Application.Auth.Dto;

public class AuthResponseDto
{
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public UserDto User { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; }
}
