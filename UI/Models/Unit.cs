namespace GuildManager.Models;

public class Unit
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public int Level { get; set; }

    public int Cost { get; set; }
}