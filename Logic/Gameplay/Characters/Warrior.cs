namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Warrior class.
// Base look: heavy grey/brown armor, shield, rust tones.
public sealed class Warrior : Adventurer
{
    public Warrior(string name) : base(name, healthPoints: 40)
    {
        BaseSuccessRate = 45; // generic recruit: 35-45%, high end of the range
    }
}