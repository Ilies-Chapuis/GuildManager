namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Mage class.
// Base look: teal/violet robe, staff, runic symbols.
public sealed class Mage : Adventurer
{
    public Mage(string name) : base(name, healthPoints: 22)
    {
        BaseSuccessRate = 35; // generic recruit: 35-45%, low end of the range
    }
}