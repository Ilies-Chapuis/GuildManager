using GuildManager.Api.Data.Entities;

namespace GuildManager.Api.Data;

public class QuestAttemptResult
{
    public bool WasSuccessful { get; set; }
    public int EstimatedSuccessRate { get; set; }
    public string Message { get; set; } = string.Empty;
}

public static class QuestResolutionService
{
    private static readonly Random Rng = new();
    private const int DeathThreshold = 30;
    private const double BonusPerExtraRecruit = 8.0;
    private const double SoloPenalty = 15.0;

    public static QuestAttemptResult Attempt(GuildEntity guild, QuestEntity quest, List<AdventurerEntity> team)
    {
        // Team size
        if (team.Count < quest.MinimumTeamSize || team.Count > quest.MaximumTeamSize)
            return Fail($"Team size must be between {quest.MinimumTeamSize} and {quest.MaximumTeamSize}.");

        // Boss requirement: at least one Warrior, one Healer, one Mage (Special adventurers never count)
        if (quest.RequiresAllRecruitCategories)
        {
            bool hasWarrior = team.Any(a => a.Category == "Warrior");
            bool hasHealer = team.Any(a => a.Category == "Healer");
            bool hasMage = team.Any(a => a.Category == "Mage");
            if (!hasWarrior || !hasHealer || !hasMage)
                return Fail("This quest requires at least one Warrior, one Healer and one Mage.");
        }

        // Specific class requirement (Special adventurers never count)
        if (quest.RequiredClassName is not null && !team.Any(a => a.Category == quest.RequiredClassName))
            return Fail($"This quest requires at least one {quest.RequiredClassName}.");

        // Level eligibility
        if (team.Any(a => a.Level < quest.MinimumLevelRequired))
            return Fail("At least one recruit does not meet the required level.");

        // Once-per-day usage
        if (team.Any(a => a.UsedToday))
            return Fail("At least one recruit has already been on a quest today.");

        // Hour budget
        if (quest.DurationHours > guild.RemainingHours)
            return Fail("Not enough hours left today for this quest.");

        guild.RemainingHours -= quest.DurationHours;

        // Success rate calculation (mirrors QuestResolver.CalculateFinalRate)
        var fighters = team.Where(a => a.Category != "Healer").ToList();
        var supports = team.Where(a => a.Category == "Healer").ToList();

        int estimatedRate;
        bool wasSuccessful;

        if (fighters.Count == 0)
        {
            estimatedRate = 0;
            wasSuccessful = false;
        }
        else
        {
            double averageRate = fighters.Average(a => IndividualSuccessRate(a, quest.Difficulty));
            double rate = averageRate + (team.Count - 1) * BonusPerExtraRecruit;

            if (team.Count == 1 && team[0].Category != "Special")
                rate -= SoloPenalty;

            foreach (var _ in supports)
                rate *= 1.2; // Healer.GroupMultiplier

            rate += FoodModifier(guild.Food);

            estimatedRate = (int)Math.Clamp(Math.Round(rate), 5, 95);
            wasSuccessful = Rng.Next(1, 101) <= estimatedRate;
        }

        // XP: same amount whether "no fighter" or "failed with a fighter"
        int xpGained = wasSuccessful ? quest.Difficulty : quest.Difficulty / 2;
        foreach (var recruit in team)
            GainExperience(recruit, xpGained);

        // Outcome
        if (wasSuccessful)
        {
            guild.Gold += quest.GoldReward;
            foreach (var recruit in team)
                recruit.UsedToday = true;
        }
        else if (estimatedRate < DeathThreshold)
        {
            foreach (var recruit in team)
            {
                if (recruit.Category == "Special")
                {
                    recruit.Status = "Injured";
                    recruit.UsedToday = true;
                }
                else
                {
                    recruit.Status = "Dead"; // soft delete, row is kept
                }
            }
        }
        else
        {
            foreach (var recruit in team)
            {
                recruit.Status = "Injured";
                recruit.UsedToday = true;
            }
        }

        quest.IsResolved = true;
        quest.WasSuccessful = wasSuccessful;

        return new QuestAttemptResult
        {
            WasSuccessful = wasSuccessful,
            EstimatedSuccessRate = estimatedRate,
            Message = wasSuccessful ? "Quest succeeded." : "Quest failed."
        };
    }

    private static int IndividualSuccessRate(AdventurerEntity adventurer, int questDifficulty)
    {
        var (_, baseRate) = AdventurerStatsCatalog.GetBaseStats(adventurer.Category, adventurer.SpecialKey);
        double rate = baseRate + (adventurer.Level - questDifficulty) * 1.3;
        return (int)Math.Clamp(Math.Round(rate), 5, 95);
    }

    private static double FoodModifier(int food)
    {
        if (food <= 0) return -25.0;
        if (food < 7) return -10.0;
        if (food > 20) return 10.0;
        return 0.0;
    }

    private static void GainExperience(AdventurerEntity recruit, int amount)
    {
        if (amount <= 0) return;

        recruit.Experience += amount;
        int threshold = recruit.Level * 100;

        while (recruit.Experience >= threshold)
        {
            recruit.Experience -= threshold;
            recruit.Level++;
            threshold = recruit.Level * 100;
        }
    }

    private static QuestAttemptResult Fail(string message) => new()
    {
        WasSuccessful = false,
        EstimatedSuccessRate = 0,
        Message = message
    };
}