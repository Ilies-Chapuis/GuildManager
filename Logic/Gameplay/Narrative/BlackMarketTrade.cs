using System;

namespace GuildManager.Logic.Gameplay.Narrative;

// The three answers to the Black Market Donkey's riddle (see
// ImprobableNpc.BlackMarketDonkey and the "donkey_riddle_day4" dialogue
// entry, triggered on day 4). Two answers are "correct" in their own way
// and reward the guild; the third is a trick answer and the donkey makes
// off with both gold and food. Unlike a plain dialogue line, picking one of
// these actually changes the guild's resources - this is the requested
// "dialogue choice with a real impact on the game" (as opposed to a choice
// that only changes what text is shown).
public enum DonkeyRiddleAnswer
{
    GoldAnswer,
    FoodAnswer,
    WrongAnswer
}

// Result of answering the riddle: how much gold/food actually moved (a
// penalty can be smaller than the nominal amount if the guild didn't have
// enough to begin with), plus a flavor line to show the player.
public readonly struct DonkeyRiddleResult
{
    public int GoldChange { get; init; }
    public int FoodChange { get; init; }
    public bool WasTricked { get; init; }
    public string FlavorText { get; init; }
}

public static class BlackMarketTrade
{
    public const int GoldReward = 80;
    public const int FoodReward = 15;

    // Nominal penalty on the wrong answer; actually taken amount is capped
    // to what the guild currently has (never goes negative).
    public const int GoldStolenOnWrongAnswer = 50;
    public const int FoodStolenOnWrongAnswer = 10;

    // The three answer button labels shown to the player. Kept here rather
    // than in dialogues.json since they're tied 1:1 to the mechanical
    // outcome below - one source of truth for what's said and what happens.
    public const string GoldAnswerText = "« Je dirais... de l'or. »";
    public const string FoodAnswerText = "« Non, clairement de la nourriture. »";
    public const string WrongAnswerText = "« Aucune idée. Surprends-moi. »";

    // Applies the chosen answer's outcome to the guild's resources and
    // returns a result carrying both the numbers and a line to display.
    public static DonkeyRiddleResult Apply(Guild guild, DonkeyRiddleAnswer answer)
    {
        switch (answer)
        {
            case DonkeyRiddleAnswer.GoldAnswer:
                guild.Resources.AddGold(GoldReward);
                return new DonkeyRiddleResult
                {
                    GoldChange = GoldReward,
                    FoodChange = 0,
                    WasTricked = false,
                    FlavorText = $"« Correct ! Enfin... plus ou moins. » L'Âne glisse {GoldReward} pièces d'or dans ta bourse."
                };

            case DonkeyRiddleAnswer.FoodAnswer:
                guild.Resources.AddFood(FoodReward);
                return new DonkeyRiddleResult
                {
                    GoldChange = 0,
                    FoodChange = FoodReward,
                    WasTricked = false,
                    FlavorText = $"« Pas faux non plus ! » L'Âne te tend {FoodReward} rations, provenance douteuse."
                };

            case DonkeyRiddleAnswer.WrongAnswer:
            default:
                int goldTaken = Math.Min(guild.Resources.Gold, GoldStolenOnWrongAnswer);
                int foodTaken = Math.Min(guild.Resources.Food, FoodStolenOnWrongAnswer);
                guild.Resources.SpendGold(goldTaken);
                guild.Resources.ConsumeFood(foodTaken);
                return new DonkeyRiddleResult
                {
                    GoldChange = -goldTaken,
                    FoodChange = -foodTaken,
                    WasTricked = true,
                    FlavorText = $"« Mauvaise réponse ! Enfin, y en avait pas de bonne. » " +
                                 $"L'Âne détale avec {goldTaken} or et {foodTaken} rations."
                };
        }
    }
}
