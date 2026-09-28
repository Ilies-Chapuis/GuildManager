namespace GuildManager.Api.Dtos;

public class CreateGuildRequestDto
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
}