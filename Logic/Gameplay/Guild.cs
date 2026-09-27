using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;
using GuildManager.Logic.Gameplay.Quests;
using GuildManager.Logic.Gameplay.Resources;

namespace GuildManager.Logic.Gameplay;

// Aggregates the full state of a playthrough

public enum EndingType
{
    None,
    Good,
    Bad
}

public sealed class Guild
{
    public const int HpRestoredPerPotion = 15;

    // Below this estimated success rate, a failed quest kills the whole
    // team instead of merely injuring it.
    public const int DeathThreshold = 30;

    // "Debt collector" end condition (GDD): evaluated once day 10 is
    // reached, not a quest of its own.
    private const int DebtDeadlineDay = 10;
    private const int DebtThreshold = 5000;

    // Reputation swing on a quest's outcome. A failed quest costs the
    // guild standing, which QuestGenerator uses to scale down future gold
    // rewards - the concrete "a quest impacts the rest of the game" link
    // requested by the professor's checklist.
    private const int ReputationGainOnSuccess = 5;
    private const int ReputationLossOnFailure = 10;

    public List<Adventurer> Roster { get; } = new();
    public GuildResources Resources { get; } = new();
    public DayCycle Cycle { get; } = new();
    public NarrativeManager Narration { get; } = new();
    public RecruitNamePool NamePool { get; } = new();

    // Adds a recruit to the guild's roster.
    public void RecruitAdventurer(Adventurer recruit) => Roster.Add(recruit);

    // Applies the day's food cost based on roster size (GDD 7.1).
    public bool ApplyDailyUpkeep()
    {
        int cost = UpkeepCalculator.CalculateFoodCost(Roster.Count);
        return Resources.ConsumeFood(cost);
    }

    // Clears every recruit's daily usage flag. Call this once at the start
    // of each new day (e.g. right after Cycle.AdvanceToNextDay()).
    public void ResetDailyUsage()
    {
        foreach (var recruit in Roster)
            recruit.ResetDailyUsage();
    }

    // Assigns a group of recruits to a quest and resolves it if the team
    // meets the quest's requirements, eligibility, the once-per-day usage
    // rule, and the day's hour budget (GDD 6.5). On success, the guild
    // gains gold, reputation, and a random loot item (see LootTable). On
    // failure, the outcome depends on the estimated success rate that was
    // actually sent: below DeathThreshold, the whole team dies (removed
    // from the roster); otherwise the team survives but returns injured (a
    // 15-point malus applies to each of them until healed with a potion).
    // Either way, a failure costs the guild reputation.
    public bool AttemptQuest(Quest quest, IReadOnlyList<Adventurer> assignedRecruits)
    {
        if (!quest.MeetsTeamRequirements(assignedRecruits))
            return false; // team too small/large, or missing a required class

        if (assignedRecruits.Any(r => !quest.IsAdventurerEligible(r)))
            return false; // at least one recruit doesn't meet the required level

        if (assignedRecruits.Any(r => r.UsedToday))
            return false; // at least one recruit here already went on a quest today

        // Checked (without consuming) before ConsumeHours, so a food
        // shortfall never wastes hours the team didn't actually use.
        if (Resources.Food < quest.FoodCost)
            return false; // not enough food to provision the team for this quest

        if (!Cycle.ConsumeHours(quest.DurationHours))
            return false; // not enough hours left today

        Resources.ConsumeFood(quest.FoodCost);

        int estimatedRate = QuestResolver.PreviewSuccessRate(quest, assignedRecruits, Resources.Food);
        bool wasSuccessful = QuestResolver.Resolve(quest, assignedRecruits, Resources.Food);

        if (wasSuccessful)
        {
            Resources.AddGold(quest.GoldReward);
            Resources.AddReputation(ReputationGainOnSuccess);

            string? loot = LootTable.DrawLoot(quest.Type);
            if (loot is not null)
                Resources.AddLoot(loot);

            foreach (var recruit in assignedRecruits)
                recruit.MarkUsedToday();
        }
        else if (estimatedRate < DeathThreshold)
        {
            // The team dies - except special adventurers, who are immune
            // to death and merely come back injured instead.
            Resources.AddReputation(-ReputationLossOnFailure);

            foreach (var recruit in assignedRecruits)
            {
                if (recruit is SpecialAdventurer)
                {
                    recruit.MarkInjured();
                    recruit.MarkUsedToday();
                }
                else
                {
                    Roster.Remove(recruit);
                }
            }
        }
        else
        {
            // The team survives but comes back injured.
            Resources.AddReputation(-ReputationLossOnFailure);

            foreach (var recruit in assignedRecruits)
            {
                recruit.MarkInjured();
                recruit.MarkUsedToday();
            }
        }

        return wasSuccessful;
    }

    // Convenient overload to assign a single recruit to a quest.
    public bool AttemptQuest(Quest quest, Adventurer recruit) => AttemptQuest(quest, new[] { recruit });

    // Consumes a health potion to heal a recruit by 10 HP (and cure the
    // injured status); fails if the potion stock is empty.
    public bool UseHealthPotion(Adventurer recruit)
    {
        if (!Resources.ConsumeHealthPotion())
            return false;

        recruit.Heal(HpRestoredPerPotion);
        return true;
    }

    // Evaluates the end-of-run "debt collector" condition once day 10 is
    // reached: the guild closes on a bad ending unless it holds at least
    // DebtThreshold gold. Standalone mechanic, not a Quest/QuestType.
    public EndingType EvaluateEnding()
    {
        if (Cycle.CurrentDay < DebtDeadlineDay)
            return EndingType.None;

        return Resources.Gold >= DebtThreshold ? EndingType.Good : EndingType.Bad;
    }
}
