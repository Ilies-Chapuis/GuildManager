namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Mage class.
// Base look: teal/violet robe, staff, runic symbols.
// FR : Recrue générique — classe Mage. Silhouette : robe teal/violette, bâton, runes.
public sealed class Mage : Adventurer
{
    // Creates a Mage recruit with fixed base stats.
    // FR : Crée une recrue Mage avec des statistiques de base fixes.
    public Mage(string name) : base(name, healthPoints: 22)
    {
        BaseSuccessRate = 52; // fragile but effective against the miasma
    }
}
