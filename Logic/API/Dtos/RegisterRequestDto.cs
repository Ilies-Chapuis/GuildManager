namespace GuildManager.Logic.API.Dtos;

// Request body for POST /auth/register.
public sealed class RegisterRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
