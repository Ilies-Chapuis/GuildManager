namespace GuildManager.Api.Dtos;

public class AdventurerStateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SpecialKey { get; set; }
    public int CurrentHealthPoints { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool UsedToday { get; set; }
    public string OwnerUsername { get; set; } = string.Empty;
}