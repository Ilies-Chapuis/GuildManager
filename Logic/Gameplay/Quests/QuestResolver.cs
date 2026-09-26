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
// FR : Résout l'issue d'une quête. Partir seul applique un malus fixe. Le
// stock de nourriture de la guilde influence aussi le taux final.
// PreviewSuccessRate calcule le même taux sans effet de bord.
public static class QuestResolver
{
    private static readonly Random RandomGenerator = new();

    private const double BonusPerExtraRecruit = 8.0;

    // Flat penalty applied when a single recruit is sent alone: no backup
    // if the quest goes wrong.
    // FR : Malus fixe quand une seule recrue part seule : aucun soutien en cas de pépin.
    private const double SoloPenalty = 15.0;

    // Food-based modifiers on the guild's success rate.
    // FR : Modificateurs liés au stock de nourriture de la guilde.
    private const int FoodBonusThreshold = 20;   // above this: bonus
    private const int FoodDebuffThreshold = 7;   // below this (but not 0): debuff
    private const double FoodBonusAmount = 10.0;
    private const double FoodDebuffAmount = 10.0;
    private const double FoodCriticalDebuffAmount = 25.0; // when food is exactly 0

    // Convenient overload to assign a single recruit to a quest.
    // FR : Surcharge pratique pour assigner une seule recrue à une quête.
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
            int consolationXp = quest.Difficulty / 2;
            foreach (var recruit in assignedRecruits)
                recruit.GainExperience(consolationXp);

            quest.MarkResolved(wasSuccessful: false);
            return false;
        }

        bool wasSuccessful = RandomGenerator.Next(1, 101) <= finalRate;

        int xpGained = wasSuccessful ? quest.Difficulty : quest.Difficulty / 2;
        foreach (var recruit in assignedRecruits)
            recruit.GainExperience(xpGained);

        quest.MarkResolved(wasSuccessful);
        return wasSuccessful;
    }

    // Computes the estimated success rate for a team given the guild's
    // current food stock, with no side effect at all (no XP, no
    // resolution) - meant to be shown to the player before they confirm
    // sending the team.
    // FR : Calcule le taux de réussite estimé, en tenant compte du stock de
    // nourriture, sans aucun effet de bord.
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
        // FR : Partir seul est plus risqué pour une recrue générique. Les
        // personnages spéciaux en sont exemptés : leur fiabilité élevée
        // (80-85%) explique justement qu'ils n'ont pas besoin de soutien.
        if (team.Count == 1 && team[0] is not SpecialAdventurer)
            rateWithBonus -= SoloPenalty;

        foreach (var support in supports)
            rateWithBonus *= support.GroupMultiplier;

        rateWithBonus += GetFoodModifier(currentFood);

        return (int)Math.Clamp(Math.Round(rateWithBonus), 5, 95);
    }

    // Bonus above FoodBonusThreshold, debuff below FoodDebuffThreshold, and
    // a much larger debuff when food has run out entirely.
    // FR : Bonus au-delà du seuil haut, malus en dessous du seuil bas, et
    // gros malus quand la nourriture est totalement épuisée.
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