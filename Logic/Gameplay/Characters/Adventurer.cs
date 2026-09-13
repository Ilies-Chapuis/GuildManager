using System;

namespace GuildManager.Logic.Gameplay.Characters;


public abstract class Adventurer
{
    public string Nom { get; }
    public int PointsDeVie { get; protected set; }
    public int Niveau { get; private set; } = 1;
    public int Experience { get; private set; }

    // les taux de réussite de basse 
    protected int TauxReussiteBase { get; set; } = 50;

    protected Adventurer(string nom, int pointsDeVie)
    {
        Nom = nom;
        PointsDeVie = pointsDeVie;
    }


    public void GagnerExperience(int xpGagne)
    {
        if (xpGagne <= 0) return;

        Experience += xpGagne;
        int seuilNiveauSuivant = Niveau * 100;

        while (Experience >= seuilNiveauSuivant)
        {
            Experience -= seuilNiveauSuivant;
            Niveau++;
            seuilNiveauSuivant = Niveau * 100;
        }
    }

    public virtual int CalculerTauxReussite(int difficulteQuete)
    {
        double taux = TauxReussiteBase + (Niveau - difficulteQuete) * 1.3;
        return Math.Clamp((int)Math.Round(taux), 5, 95);
    }

    public override string ToString() => $"{Nom} (Niv. {Niveau}, {PointsDeVie} PV)";
}
