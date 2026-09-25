using System;

namespace GuildManager.Api.Data.Entities;

public class QuestEntity
{
    public int Id { get; set; }
    public int GuildId { get; set; }
    public int? OwnerPlayerId { get; set; } // null = guild-wide/shared quest

    public string Name { get; set; } = string.Empty;
    public int DayAvailable { get; set; }
    public string Type { get; set; } = string.Empty;

    public int Difficulty { get; set; }
    public int DurationHours { get; set; }
    public int GoldReward { get; set; }

    public int MinimumLevelRequired { get; set; }
    public int MinimumTeamSize { get; set; }
    public int MaximumTeamSize { get; set; }

    public bool RequiresAllRecruitCategories { get; set; }
    public string? RequiredClassName { get; set; }

    public bool IsResolved { get; set; }
    public bool? WasSuccessful { get; set; }

    public DateTime CreatedAt { get; set; }

    public GuildEntity? Guild { get; set; }
    public PlayerEntity? OwnerPlayer { get; set; }
}