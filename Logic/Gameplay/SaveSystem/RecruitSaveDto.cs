namespace GuildManager.Logic.Gameplay.SaveSystem;

// Saved state of one recruit (generic or special). "Type" identifies which
// C# class to recreate via AdventurerFactory (Warrior/Healer/Mage for
// generics, Sameth/Meloap/Elowen for specials).
// FR : État sauvegardé d'une recrue. "Type" identifie la classe C# à
// recréer via AdventurerFactory.
public sealed class RecruitSaveDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int HealthPoints { get; set; }
}
