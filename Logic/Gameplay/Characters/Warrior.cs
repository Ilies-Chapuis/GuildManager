namespace GuildManager.Logic.Gameplay.Characters;

/// <summary>
/// Recrue générique — classe Guerrier (GDD 5.4).
/// Silhouette de base : armure lourde grise/brune, bouclier, teintes rouille.
/// </summary>
public sealed class Warrior : Adventurer
{
    public Warrior(string nom) : base(nom, pointsDeVie: 40)
    {
        TauxReussiteBase = 55; 
    }
}
