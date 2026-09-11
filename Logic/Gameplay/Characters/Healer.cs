namespace GuildManager.Logic.Gameplay.Characters;

/// <summary>
/// Recrue générique — classe Healeur (GDD 5.4).
/// Silhouette de base : robe blanche/or, symbole sacré, orbe de soin.
/// </summary>
public sealed class Healer : Adventurer
{
    public Healer(string nom) : base(nom, pointsDeVie: 28)
    {
        TauxReussiteBase = 50; 
    }
}
