namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Healer class.
// Base look: white/gold robe, sacred symbol, healing orb.
// The Healer has NO success rate of its own: CalculateSuccessRate always
// returns 0 (it never fights the quest itself). Its entire specificity is
// to multiply the rest of the team's success rate by 1.3.
// FR : Le Healeur n'a AUCUN taux de réussite propre : CalculateSuccessRate
// renvoie toujours 0. Sa seule spécificité est de multiplier le taux de
// réussite du reste de l'équipe par 1.3.
public sealed class Healer : Adventurer, ISupportRecruit
{
    public double GroupMultiplier => 1.3;

    public Healer(string name) : base(name, healthPoints: 28)
    {
        // Intentionally not set: BaseSuccessRate is irrelevant here, since
        // CalculateSuccessRate is overridden below to always return 0.
        // FR : Volontairement non défini : BaseSuccessRate ne sert à rien
        // ici, CalculateSuccessRate étant toujours 0.
    }

    public override int CalculateSuccessRate(int questLevel) => 0;
}