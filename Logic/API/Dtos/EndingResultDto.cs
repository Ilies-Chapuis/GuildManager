namespace GuildManager.Logic.API.Dtos;

// Response shape for POST /guilds/{guildId}/evaluate-ending.
public sealed class EndingResultDto
{
    public int Id { get; set; }
    public string Ending { get; set; } = string.Empty;
    public int Gold { get; set; }
}
