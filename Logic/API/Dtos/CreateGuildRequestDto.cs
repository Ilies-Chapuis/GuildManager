namespace GuildManager.Logic.API.Dtos;

// Request body for POST /guilds.
public sealed class CreateGuildRequestDto
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
}
