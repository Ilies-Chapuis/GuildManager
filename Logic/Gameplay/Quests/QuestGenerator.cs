using System;
using System.Collections.Generic;

namespace GuildManager.Logic.Gameplay.Quests;

// Generates the quests available for a given day. Quest level scales with
// the day (1 on days 1-3, then +1 every two days, up to 5 on day 10),
// matching Adventurer.Level's own scale. Escort requires a Warrior,
// Exorcism requires a Healer. Extracted here (rather than staying inline in
// the TMP terminal harness) so the future WPF UI can call it directly.
public static class QuestGenerator
{
    // Reputation is clamped to this range before it scales gold rewards,
    // so a very bad (or very good) streak doesn't swing rewards wildly.
    private const int ReputationClampMin = -50;
    private const int ReputationClampMax = 50;

    // Quest level for a given day: level 1 on days 1-3, then +1 every two
    // days, reaching level 5 on day 10.
    public static int GetQuestLevelForDay(int day)
    {
        if (day <= 3) return 1;
        return 1 + (int)Math.Ceiling((day - 3) / 2.0);
    }

    // Scales a base gold reward by the guild's current reputation: a
    // failure-heavy guild (negative reputation) sees smaller rewards, a
    // reliable one (positive reputation) sees slightly better ones. This is
    // how a past quest's outcome ripples into future quests, without
    // touching quest difficulty or availability.
    private static int ScaleGoldByReputation(int baseGold, int reputation)
    {
        int clamped = Math.Clamp(reputation, ReputationClampMin, ReputationClampMax);
        double factor = 1.0 + clamped / 100.0; // -50 rep -> x0.5, +50 rep -> x1.5
        return Math.Max(10, (int)Math.Round(baseGold * factor));
    }

    // Generates the standard set of non-boss quests offered on a given day.
    // reputation defaults to 0 (neutral) so existing callers keep working
    // unchanged; pass the guild's actual Resources.Reputation to make
    // rewards reflect its track record.
    public static List<Quest> GenerateForDay(int day, int reputation = 0)
    {
        int level = GetQuestLevelForDay(day);

        // Rewards doubled (x2) compared to the previous version.
        return new List<Quest>
        {
            new("Escorte", day, QuestType.Escort,
                difficulty: level, durationHours: 4,
                goldReward: ScaleGoldByReputation(160 + day * 20, reputation),
                requiredClassName: "Warrior"),
            new("Exorcisme", day, QuestType.Exorcism,
                difficulty: level, durationHours: 6,
                goldReward: ScaleGoldByReputation(240 + day * 30, reputation),
                requiredClassName: "Healer"),
            new("Exploration de donjon", day, QuestType.DungeonExploration,
                difficulty: level, durationHours: 8,
                goldReward: ScaleGoldByReputation(300 + day * 40, reputation)),
        };
    }
}
