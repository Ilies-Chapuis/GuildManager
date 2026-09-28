using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.Logic.Gameplay.Quests;

// A quest offered to the guild. GDD 6.1: every quest has restrictions on
// which adventurers can be assigned; GDD 6.5: every quest has a duration in
// hours, charged against the day's 16-hour budget.
public sealed class Quest
{
    public string Name { get; }
    public int DayAvailable { get; }
    public QuestType Type { get; }
    public int Difficulty { get; }
    public int DurationHours { get; }
    public int GoldReward { get; }

    // Food consumed from the guild's stock to send a team on this quest:
    // 2 for a 4h quest, 3 for 6h, 4 for 8h (half the duration in hours).
    public int FoodCost => DurationHours / 2;

    // Experience granted to each participant on success (half that amount
    // on failure, as a consolation - see QuestResolver.Resolve). A base
    // amount per quest type, scaled up the later in the run the quest is
    // performed (DayAvailable) - same principle as GoldReward growing with
    // the day, so late-game quests stay worth doing even once early
    // recruits are already high level.
    public int ExperienceReward
    {
        get
        {
            (int baseXp, int perDay) = Type switch
            {
                QuestType.Escort => (50, 5),
                QuestType.Exorcism => (75, 8),
                QuestType.DungeonExploration => (100, 10),
                QuestType.BossFight => (1000, 100),
                _ => (50, 5),
            };
            return baseXp + DayAvailable * perDay;
        }
    }
    public int MinimumLevelRequired { get; }

    // Minimum/maximum number of recruits that can be assigned together.
    public int MinimumTeamSize { get; }
    public int MaximumTeamSize { get; }

    // If true, the team must contain at least one Warrior, one Healer and one Mage (boss fights).
    public bool RequiresAllRecruitCategories { get; }

    // If set, the team must contain at least one recruit of this class
    // ("Warrior", "Healer" or "Mage"). Null means no such requirement.
    public string? RequiredClassName { get; }

    // Minimum number of special adventurers (Sameth/Meloap/Elowen) required
    // in the team. Used by boss fights, which demand seasoned, reliable
    // recruits rather than a fresh generic roster.
    public int MinimumSpecialAdventurers { get; }

    public bool IsResolved { get; private set; }
    public bool WasSuccessful { get; private set; }

    public Quest(string name, int dayAvailable, QuestType type, int difficulty, int durationHours,
        int goldReward, int minimumLevelRequired = 1, int minimumTeamSize = 1,
        int maximumTeamSize = 5, bool requiresAllRecruitCategories = false,
        string? requiredClassName = null, int minimumSpecialAdventurers = 0)
    {
        Name = name;
        DayAvailable = dayAvailable;
        Type = type;
        Difficulty = difficulty;
        DurationHours = durationHours;
        GoldReward = goldReward;
        MinimumLevelRequired = minimumLevelRequired;
        MinimumTeamSize = minimumTeamSize;
        MaximumTeamSize = maximumTeamSize;
        RequiresAllRecruitCategories = requiresAllRecruitCategories;
        RequiredClassName = requiredClassName;
        MinimumSpecialAdventurers = minimumSpecialAdventurers;
    }

    // Records the outcome of the quest; can only be called once during
    // normal gameplay flow.
    public void MarkResolved(bool wasSuccessful)
    {
        IsResolved = true;
        WasSuccessful = wasSuccessful;
    }

    // Restores a previously-resolved quest's state from a save file,
    // bypassing the normal resolution flow (no "already resolved" check,
    // no side effects). Meant to be called only by the persistence layer
    // right after loading a Quest from the database.
    public void RestoreResolution(bool isResolved, bool wasSuccessful)
    {
        IsResolved = isResolved;
        WasSuccessful = wasSuccessful;
    }

    // Checks whether a single recruit meets the quest's minimum required level.
    public bool IsAdventurerEligible(Adventurer recruit) => recruit.Level >= MinimumLevelRequired;

    // Returns a human-readable reason the team cannot be assigned, or null if
    // every requirement (size, categories, required class) is satisfied.
    public string? DescribeUnmetRequirement(IReadOnlyList<Adventurer> team)
    {
        if (team.Count < MinimumTeamSize)
            return $"This quest requires at least {MinimumTeamSize} recruit(s).";

        if (team.Count > MaximumTeamSize)
            return $"At most {MaximumTeamSize} recruits can be assigned to this quest.";

        if (RequiresAllRecruitCategories)
        {
            bool hasWarrior = team.OfType<Warrior>().Any();
            bool hasHealer = team.OfType<Healer>().Any();
            bool hasMage = team.OfType<Mage>().Any();
            if (!hasWarrior || !hasHealer || !hasMage)
                return "This quest requires at least one Warrior, one Healer and one Mage.";
        }

        if (RequiredClassName is not null && !HasClass(team, RequiredClassName))
            return $"This quest requires at least one {RequiredClassName}.";

        if (MinimumSpecialAdventurers > 0)
        {
            int specialCount = team.OfType<SpecialAdventurer>().Count();
            if (specialCount < MinimumSpecialAdventurers)
                return $"This quest requires at least {MinimumSpecialAdventurers} special adventurer(s) (Sameth, Meloap or Elowen).";
        }

        return null;
    }

    private static bool HasClass(IReadOnlyList<Adventurer> team, string className) => className switch
    {
        "Warrior" => team.OfType<Warrior>().Any(),
        "Healer" => team.OfType<Healer>().Any(),
        "Mage" => team.OfType<Mage>().Any(),
        _ => true
    };

    // Checks the team-wide requirements as a simple bool (see DescribeUnmetRequirement for details).
    public bool MeetsTeamRequirements(IReadOnlyList<Adventurer> team) => DescribeUnmetRequirement(team) is null;

    // Returns a short human-readable summary of the quest.
    public override string ToString() =>
        $"{Name} [{Type}] - difficulty {Difficulty}, {DurationHours}h, {GoldReward} gold";
}