namespace GuildManager.Logic.Gameplay.Characters;

/// <summary>
/// Recrue générique — classe Mage (GDD 5.4).
/// Silhouette de base : robe teal/violette, bâton, symboles runiques.
/// </summary>
public sealed class Mage : Adventurer
{
    public Mage(string nom) : base(nom, pointsDeVie: 22)
    {
        TauxReussiteBase = 52; 
    }
}
