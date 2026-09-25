using System;
using System.Collections.Generic;

namespace GuildManager.Api.Data.Entities;

public class GuildEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentDay { get; set; }
    public int RemainingHours { get; set; }
    public int Gold { get; set; }
    public int Food { get; set; }
    public int HealthPotions { get; set; }
    public string Ending { get; set; } = "None";
    public DateTime CreatedAt { get; set; }

    public List<AdventurerEntity> Adventurers { get; set; } = new();
    public List<QuestEntity> Quests { get; set; } = new();
    public List<GuildMembershipEntity> Memberships { get; set; } = new();
}