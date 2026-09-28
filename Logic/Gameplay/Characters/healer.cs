namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Healer class.
// Base look: white/gold robe, sacred symbol, healing orb.
// The Healer has NO success rate of its own: CalculateSuccessRate always
// returns 0 (it never fights the quest itself). Its entire specificity is
// to multiply the rest of the team's success rate by 1.3.
public sealed class Healer : Adventurer, ISupportRecruit
{
    public double GroupMultiplier => 1.3;

    public Healer(string name) : base(name, healthPoints: 28)
    {
        // Intentionally not set: BaseSuccessRate is irrelevant here, since
        // CalculateSuccessRate is overridden below to always return 0.
    }

    public override int CalculateSuccessRate(int questLevel) => 0;
}