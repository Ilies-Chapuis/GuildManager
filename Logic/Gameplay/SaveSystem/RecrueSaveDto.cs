namespace GuildManager.Logic.Gameplay.SaveSystem;


public sealed class RecrueSaveDto
{
    public string Nom { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Niveau { get; set; } = 1;
    public int Experience { get; set; }
    public int PointsDeVie { get; set; }
}
