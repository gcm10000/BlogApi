namespace BlogApi.Application.Auth.Dto;

public class AuthResponseDto
{
    public string Access_Token { get; set; }
    public string Refresh_Token { get; set; }
    public UserDto User { get; set; }
}
