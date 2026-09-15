namespace GuildManager.Logic.Gameplay.Characters;


public abstract class SpecialAdventurer : Adventurer
{
    public string Histoire { get; }
    public bool EvenementUniqueDeclenche { get; private set; }

    protected SpecialAdventurer(string nom, int pointsDeVie, string histoire)
        : base(nom, pointsDeVie)
    {
        Histoire = histoire;
    }

    /// <summary>Réplique jouée quand la Voix Sérieuse est active.</summary>
    public abstract string ObtenirTexteSerieux();

    /// <summary>Réplique jouée quand la Voix WTF est active.</summary>
    public abstract string ObtenirTexteWtf();

    public void MarquerEvenementUniqueDeclenche() => EvenementUniqueDeclenche = true;
}
