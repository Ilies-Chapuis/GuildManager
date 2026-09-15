namespace GuildManager.Logic.Gameplay.Characters;


public sealed class Mage : Adventurer
{
    public Mage(string nom) : base(nom, pointsDeVie: 22)
    {
        TauxReussiteBase = 52; 
    }
}
