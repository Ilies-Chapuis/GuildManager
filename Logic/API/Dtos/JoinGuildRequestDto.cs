namespace GuildManager.Logic.API.Dtos;

// Request body for POST /guilds/{guildId}/join.
public sealed class JoinGuildRequestDto
{
    public int PlayerId { get; set; }
}
