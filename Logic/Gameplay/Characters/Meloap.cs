namespace GuildManager.Logic.Gameplay.Characters;

/// <summary>
/// Meloap — aventurier homme-lézard (GDD 5.1 / 5.3).
/// Allié indéfectible du joueur depuis ses tout premiers pas à la Guilde de Fadriann
/// (dont la fameuse porte magique du Prologue). Devient le bras droit de la guilde.
/// </summary>
public sealed class Meloap : SpecialAdventurer
{
    public Meloap() : base(
        nom: "Meloap",
        pointsDeVie: 50,
        histoire: "Aventurier homme-lézard, allié du joueur depuis ses tout premiers pas " +
                  "à la Guilde de Fadriann. Toujours prêt à soutenir les recrues sur le terrain.")
    {
        TauxReussiteBase = 88; // aventurier spécial : le plus fiable du roster (> 80%)
    }

    public override string ObtenirTexteSerieux() =>
        "Un ancien compagnon de l'expédition à Tartaros a disparu. Je crains que ce ne soit pas une coïncidence.";

    public override string ObtenirTexteWtf() =>
        "Ce cochon porte manifestement les habits d'un noble étranger. Traitons-le avec tous les honneurs.";
}
