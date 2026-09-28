using System;

namespace GuildManager.Api.Data.Entities;

public class QuestAssignmentEntity
{
    public int QuestId { get; set; }
    public int AdventurerId { get; set; }
    public DateTime AssignedAt { get; set; }

    public QuestEntity? Quest { get; set; }
    public AdventurerEntity? Adventurer { get; set; }
}