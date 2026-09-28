using System;

namespace GuildManager.Api.Data.Entities;

public class AdventurerEntity
{
    public int Id { get; set; }
    public int GuildId { get; set; }
    public int OwnerPlayerId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "Warrior"/"Healer"/"Mage"/"Special"
    public string? SpecialKey { get; set; }               // "Elowen"/"Meloap"/"Sameth", or null

    public int CurrentHealthPoints { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public bool? UniqueEventTriggered { get; set; }
    public bool UsedToday { get; set; }

    public string Status { get; set; } = "Available";
    public DateTime CreatedAt { get; set; }

    public GuildEntity? Guild { get; set; }
    public PlayerEntity? OwnerPlayer { get; set; }
}