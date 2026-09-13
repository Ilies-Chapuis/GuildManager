namespace GuildManager.Logic.Gameplay.Characters;

/// <summary>
/// Elowen — haute-elfe archère et rôdeuse (GDD 5.1 / 5.3).
/// Nouvelle recrue spéciale sans passé commun avec le joueur ; sa connexion à la
/// nature lui permet de percevoir les premiers signes du miasme avant quiconque.
/// </summary>
public sealed class Elowen : SpecialAdventurer
{
    public Elowen() : base(
        nom: "Elowen",
        pointsDeVie: 32,
        histoire: "Haute-elfe archère et rôdeuse, rejoint la guilde comme sentinelle " +
                  "discrète capable de détecter les prémices du miasme dans la végétation.")
    {
        TauxReussiteBase = 85; // aventurière spéciale : fiabilité élevée (> 80%)
    }

    public override string ObtenirTexteSerieux() =>
        "La forêt murmure des choses inquiétantes, ces derniers temps. Le miasme progresse plus vite qu'il ne devrait.";

    public override string ObtenirTexteWtf() =>
        "Le Pigeon dit qu'il porte un message royal urgent. Je le traduis fidèlement : il ment, mais avec panache.";
}
