namespace GuildManager.Api.Dtos;

public class RecruitAdventurerRequestDto
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "Warrior"/"Healer"/"Mage"/"Special"
    public string? SpecialKey { get; set; }               // required if Category == "Special"
}