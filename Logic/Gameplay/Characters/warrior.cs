namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Warrior class.
// Base look: heavy grey/brown armor, shield, rust tones.
// FR : Recrue générique — classe Guerrier. Silhouette : armure lourde grise/brune, bouclier.
public sealed class Warrior : Adventurer
{
    public Warrior(string name) : base(name, healthPoints: 40)
    {
        BaseSuccessRate = 45; // recrue générique : 35-45%, valeur haute de la fourchette
    }
}