namespace GuildManager.Logic.Gameplay.Characters;

public sealed class Healer : Adventurer, ISupportRecruit
{
    public double MultiplicateurGroupe => 1.2;

    public Healer(string nom) : base(nom, pointsDeVie: 28)
    {
        TauxReussiteBase = 0; 
    }

    public override int CalculerTauxReussite(int difficulteQuete) => 0;
}
