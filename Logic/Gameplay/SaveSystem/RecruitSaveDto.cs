namespace GuildManager.Logic.Gameplay.SaveSystem;

// Saved state of one recruit (generic or special). "Type" identifies which
// C# class to recreate via AdventurerFactory (Warrior/Healer/Mage for
// generics, Sameth/Meloap/Elowen for specials). UniqueEventTriggered only
// matters for special adventurers; it stays false for generic recruits.
// FR : État sauvegardé d'une recrue. UniqueEventTriggered ne compte que pour
// les personnages spéciaux ; reste false pour les recrues génériques.
public sealed class RecruitSaveDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int HealthPoints { get; set; }
    public bool UniqueEventTriggered { get; set; }
}
