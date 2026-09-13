namespace GuildManager.Logic.Gameplay.Characters;

/// <summary>
/// Sameth — marchand-mage repenti (GDD 5.1 / 5.3).
/// Ami de longue date du joueur, révélé possédé par Hedge durant le Prologue
/// (l'expédition à Tartaros), responsable de l'attaque contre l'ancienne Guilde.
/// Une fois exorcisé, il rejoint la nouvelle guilde comme marchand et conseiller.
/// </summary>
public sealed class Sameth : SpecialAdventurer
{
    public Sameth() : base(
        nom: "Sameth",
        pointsDeVie: 30,
        histoire: "Marchand-mage repenti, jadis possédé par Hedge lors de l'attaque de " +
                  "l'ancienne Guilde. Exorcisé au sommet de Tartaros, il rejoint la nouvelle " +
                  "guilde comme marchand et conseiller, hanté par la culpabilité de ce qu'il " +
                  "a fait sous l'emprise du miasme.")
    {
        TauxReussiteBase = 82; // aventurier spécial : fiabilité élevée, indépendante du niveau (> 80%)
    }

    public override string ObtenirTexteSerieux() =>
        "Je reconnais ce symbole... c'est celui de Hedge. Le miasme n'a jamais vraiment disparu.";

    public override string ObtenirTexteWtf() =>
        "Regardez ces reliques authentiques ! Enfin, authentiques depuis hier soir, mais qui vérifie vraiment ?";
}
