namespace GuildManager.Logic.API.Dtos;

// Response shape for POST /guilds/{guildId}/advance-day.
public sealed class DayAdvanceResultDto
{
    public int Id { get; set; }
    public int CurrentDay { get; set; }
    public int RemainingHours { get; set; }
}
