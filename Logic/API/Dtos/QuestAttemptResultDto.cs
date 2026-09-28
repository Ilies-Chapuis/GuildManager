namespace GuildManager.Logic.API.Dtos;

// Response shape for POST /quests/{questId}/attempt.
public sealed class QuestAttemptResultDto
{
    public bool WasSuccessful { get; set; }
    public int EstimatedSuccessRate { get; set; }
    public string Message { get; set; } = string.Empty;
}
