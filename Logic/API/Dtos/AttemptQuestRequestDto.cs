namespace GuildManager.Logic.API.Dtos;

// Request body for POST /quests/{questId}/attempt.
public sealed class AttemptQuestRequestDto
{
    public List<int> AdventurerIds { get; set; } = new();
}
