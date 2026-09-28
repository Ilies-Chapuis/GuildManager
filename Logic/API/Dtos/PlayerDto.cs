namespace GuildManager.Logic.API.Dtos;

// Response shape for /auth/register and /auth/login (see GuildManager.Api's
// Program.cs: Results.Ok(new { player.Id, player.Username, player.Email })).
public sealed class PlayerDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
