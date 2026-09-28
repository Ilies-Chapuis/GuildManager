namespace GuildManager.Api.Dtos;

public class QuestStateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public int DurationHours { get; set; }
    public int GoldReward { get; set; }
    public int MinimumTeamSize { get; set; }
    public int MaximumTeamSize { get; set; }
    public bool RequiresAllRecruitCategories { get; set; }
    public string? RequiredClassName { get; set; }
    public int DayAvailable { get; set; }
    public bool IsResolved { get; set; }
    public bool? WasSuccessful { get; set; }
    public int? OwnerPlayerId { get; set; }
}