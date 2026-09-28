using System;
using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.Logic.Gameplay.Quests;

// Resolves a quest's outcome based on difficulty, team size, and the guild's
// current food stock. Support recruits (ISupportRecruit, e.g. Healer) don't
// fight themselves: they multiply the group's rate, but a group made only of
// supports fails automatically (0%). Going solo carries a flat penalty -
// there is no backup if things go wrong. PreviewSuccessRate exposes the same
// math with no side effect, so the UI can show the estimated rate before the
// team is actually sent.
public static class QuestResolver
{
    private static readonly Random RandomGenerator = new();

    private const double BonusPerExtraRecruit = 8.0;

    // Flat penalty applied when a single recruit is sent alone: no backup
    // if the quest goes wrong.
    private const double SoloPenalty = 15.0;

    // Boss fights are meant to be genuinely hard even for a well-built team:
    // this penalty is applied on top of everything else, representing the
    // boss's raw power rather than anything the team composition can offset.
    private const double BossFightPenalty = 40.0;

    // Food-based modifiers on the guild's success rate.
    private const int FoodBonusThreshold = 20;   // above this: bonus
    private const int FoodDebuffThreshold = 7;   // below this (but not 0): debuff
    private const double FoodBonusAmount = 10.0;
    private const double FoodDebuffAmount = 10.0;
    private const double FoodCriticalDebuffAmount = 25.0; // when food is exactly 0

    // Convenient overload to assign a single recruit to a quest.
    public static bool Resolve(Quest quest, Adventurer assignedRecruit, int currentFood) =>
        Resolve(quest, new[] { assignedRecruit }, currentFood);

    public static bool Resolve(Quest quest, IReadOnlyList<Adventurer> assignedRecruits, int currentFood)
    {
        if (quest.IsResolved)
            throw new InvalidOperationException($"Quest {quest.Name} has already been resolved.");

        if (assignedRecruits.Count == 0)
            throw new ArgumentException("At least one recruit must be assigned to the quest.");

        int finalRate = CalculateFinalRate(quest, assignedRecruits, currentFood, out bool hasFighter);

        if (!hasFighter)
        {
            // Only supports assigned: nobody actually fights the quest.
            int consolationXp = quest.ExperienceReward / 2;
            foreach (var recruit in assignedRecruits)
                recruit.GainExperience(consolationXp);

            quest.MarkResolved(wasSuccessful: false);
            return false;
        }

        bool wasSuccessful = RandomGenerator.Next(1, 101) <= finalRate;

        int xpGained = wasSuccessful ? quest.ExperienceReward : quest.ExperienceReward / 2;
        foreach (var recruit in assignedRecruits)
            recruit.GainExperience(xpGained);

        quest.MarkResolved(wasSuccessful);
        return wasSuccessful;
    }

    // Computes the estimated success rate for a team given the guild's
    // current food stock, with no side effect at all (no XP, no
    // resolution) - meant to be shown to the player before they confirm
    // sending the team.
    public static int PreviewSuccessRate(Quest quest, IReadOnlyList<Adventurer> team, int currentFood) =>
        CalculateFinalRate(quest, team, currentFood, out _);

    private static int CalculateFinalRate(Quest quest, IReadOnlyList<Adventurer> team, int currentFood, out bool hasFighter)
    {
        var fighters = team.Where(r => r is not ISupportRecruit).ToList();
        var supports = team.OfType<ISupportRecruit>().ToList();

        hasFighter = fighters.Count > 0;
        if (!hasFighter)
            return 0;

        double averageRate = fighters.Average(r => r.CalculateSuccessRate(quest.Difficulty));
        double teamBonus = (team.Count - 1) * BonusPerExtraRecruit;
        double rateWithBonus = averageRate + teamBonus;

        // Going solo is riskier for a generic recruit - no one to pick up
        // the slack. Special adventurers are exempt: their high reliability
        // (80-85%) is precisely why they don't need backup.
        if (team.Count == 1 && team[0] is not SpecialAdventurer)
            rateWithBonus -= SoloPenalty;

        foreach (var support in supports)
            rateWithBonus *= support.GroupMultiplier;

        if (quest.Type == QuestType.BossFight)
            rateWithBonus -= BossFightPenalty;

        rateWithBonus += GetFoodModifier(currentFood);

        return (int)Math.Clamp(Math.Round(rateWithBonus), 5, 95);
    }

    // Bonus above FoodBonusThreshold, debuff below FoodDebuffThreshold, and
    // a much larger debuff when food has run out entirely.
    private static double GetFoodModifier(int currentFood)
    {
        if (currentFood <= 0)
            return -FoodCriticalDebuffAmount;

        if (currentFood < FoodDebuffThreshold)
            return -FoodDebuffAmount;

        if (currentFood > FoodBonusThreshold)
            return FoodBonusAmount;

        return 0;
    }
}