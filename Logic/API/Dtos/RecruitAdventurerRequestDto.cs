namespace GuildManager.Logic.API.Dtos;

// Request body for POST /guilds/{guildId}/adventurers.
// Category is "Warrior"/"Healer"/"Mage"/"Special"; SpecialKey (e.g.
// "Sameth"/"Meloap"/"Elowen") is required only when Category == "Special".
public sealed class RecruitAdventurerRequestDto
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SpecialKey { get; set; }
}
