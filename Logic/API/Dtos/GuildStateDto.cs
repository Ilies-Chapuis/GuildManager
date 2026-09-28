namespace GuildManager.Logic.API.Dtos;

// Response shape returned by every guild endpoint (create/join/get).
public sealed class GuildStateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentDay { get; set; }
    public int RemainingHours { get; set; }
    public int Gold { get; set; }
    public int Food { get; set; }
    public int HealthPotions { get; set; }
    public string Ending { get; set; } = string.Empty;
    public List<string> MemberUsernames { get; set; } = new();
}
