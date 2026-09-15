namespace GuildManager.Logic.Gameplay.Characters;


public sealed class Warrior : Adventurer
{
    public Warrior(string nom) : base(nom, pointsDeVie: 40)
    {
        TauxReussiteBase = 55; 
    }
}
