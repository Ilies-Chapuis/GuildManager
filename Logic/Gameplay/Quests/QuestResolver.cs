using System;
using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.Logic.Gameplay.Quests;

// Resolves a quest's outcome based on difficulty and the number of recruits
// assigned (GDD 6.4). Every extra recruit adds a team bonus. Support recruits
// (ISupportRecruit, e.g. Healer) don't fight themselves: they multiply the
// group's rate, but a group made only of supports fails automatically (0%).
// FR : Résout l'issue d'une quête selon la difficulté et le nombre de recrues
// assignées. Un groupe composé uniquement de soutiens échoue automatiquement.
public static class QuestResolver
{
    private static readonly Random RandomGenerator = new();
    private const double BonusPerExtraRecruit = 8.0;

    // Convenient overload to assign a single recruit to a quest.
    // FR : Surcharge pratique pour assigner une seule recrue à une quête.
    public static bool Resolve(Quest quest, Adventurer assignedRecruit) =>
        Resolve(quest, new[] { assignedRecruit });

    // Resolves the quest for a group of assigned recruits and applies XP/gold.
    // FR : Résout la quête pour un groupe de recrues assignées et applique XP/or.
    public static bool Resolve(Quest quest, IReadOnlyList<Adventurer> assignedRecruits)
    {
        if (quest.IsResolved)
            throw new InvalidOperationException($"Quest {quest.Name} has already been resolved.");

        if (assignedRecruits.Count == 0)
            throw new ArgumentException("At least one recruit must be assigned to the quest.");

        var fighters = assignedRecruits.Where(r => r is not ISupportRecruit).ToList();
        var supports = assignedRecruits.OfType<ISupportRecruit>().ToList();

        if (fighters.Count == 0)
        {
            int consolationXp = quest.Difficulty / 2;
            foreach (var recruit in assignedRecruits)
                recruit.GainExperience(consolationXp);

            quest.MarkResolved(wasSuccessful: false);
            return false;
        }

        double averageRate = fighters.Average(r => r.CalculateSuccessRate(quest.Difficulty));
        double teamBonus = (assignedRecruits.Count - 1) * BonusPerExtraRecruit;
        double rateWithBonus = averageRate + teamBonus;

        foreach (var support in supports)
            rateWithBonus *= support.GroupMultiplier;

        int finalRate = (int)Math.Clamp(Math.Round(rateWithBonus), 5, 95);
        bool wasSuccessful = RandomGenerator.Next(1, 101) <= finalRate;

        int xpGained = wasSuccessful ? quest.Difficulty : quest.Difficulty / 2;
        foreach (var recruit in assignedRecruits)
            recruit.GainExperience(xpGained);

        quest.MarkResolved(wasSuccessful);
        return wasSuccessful;
    }
}
