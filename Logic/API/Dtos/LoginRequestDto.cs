namespace GuildManager.Logic.API.Dtos;

// Request body for POST /auth/login.
public sealed class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
