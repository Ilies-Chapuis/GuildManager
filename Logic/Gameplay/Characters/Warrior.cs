namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Warrior class.
// Base look: heavy grey/brown armor, shield, rust tones.
// FR : Recrue générique — classe Guerrier. Silhouette : armure lourde grise/brune, bouclier.
public sealed class Warrior : Adventurer
{
    // Creates a Warrior recruit with fixed base stats.
    // FR : Crée une recrue Guerrier avec des statistiques de base fixes.
    public Warrior(string name) : base(name, healthPoints: 40)
    {
        BaseSuccessRate = 55; // solid on direct-confrontation quests
    }
}
