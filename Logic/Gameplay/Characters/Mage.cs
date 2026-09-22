namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Mage class.
// Base look: teal/violet robe, staff, runic symbols.
// FR : Recrue générique — classe Mage. Silhouette : robe teal/violette, bâton, runes.
public sealed class Mage : Adventurer
{
    public Mage(string name) : base(name, healthPoints: 22)
    {
        BaseSuccessRate = 340; // recrue générique : 35-45%, valeur basse de la fourchette
    }
}