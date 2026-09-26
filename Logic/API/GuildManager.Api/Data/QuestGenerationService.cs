using GuildManager.Api.Data.Entities;

namespace GuildManager.Api.Data;

public static class QuestGenerationService
{
    // Mirrors QuestGenerator.GetQuestLevelForDay
    public static int GetQuestLevelForDay(int day)
    {
        if (day <= 3) return 1;
        return 1 + (int)Math.Ceiling((day - 3) / 2.0);
    }

    // One Escort/Exorcism/DungeonExploration set per player, for the guild's current day.
    // Placeholder note: boss fight numbers (day 5 & 10) are not specified in the GDD/code
    // seen so far -- adjust once the team confirms them.
    public static List<QuestEntity> GenerateForDay(GuildEntity guild, List<int> playerIds)
    {
        int day = guild.CurrentDay;
        int level = GetQuestLevelForDay(day);
        var quests = new List<QuestEntity>();

        foreach (int playerId in playerIds)
        {
            quests.Add(new QuestEntity
            {
                GuildId = guild.Id,
                OwnerPlayerId = playerId,
                Name = $"Escorte (jour {day})",
                DayAvailable = day,
                Type = "Escort",
                Difficulty = level,
                DurationHours = 4,
                GoldReward = 80 + day * 10,
                MinimumLevelRequired = 1,
                MinimumTeamSize = 1,
                MaximumTeamSize = 5,
                RequiredClassName = "Warrior",
                CreatedAt = DateTime.UtcNow
            });

            quests.Add(new QuestEntity
            {
                GuildId = guild.Id,
                OwnerPlayerId = playerId,
                Name = $"Exorcisme (jour {day})",
                DayAvailable = day,
                Type = "Exorcism",
                Difficulty = level,
                DurationHours = 6,
                GoldReward = 120 + day * 15,
                MinimumLevelRequired = 1,
                MinimumTeamSize = 1,
                MaximumTeamSize = 5,
                RequiredClassName = "Healer",
                CreatedAt = DateTime.UtcNow
            });

            quests.Add(new QuestEntity
            {
                GuildId = guild.Id,
                OwnerPlayerId = playerId,
                Name = $"Exploration de donjon (jour {day})",
                DayAvailable = day,
                Type = "DungeonExploration",
                Difficulty = level,
                DurationHours = 8,
                GoldReward = 150 + day * 20,
                MinimumLevelRequired = 1,
                MinimumTeamSize = 1,
                MaximumTeamSize = 5,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Shared boss fight on day 5 and day 10 -- PLACEHOLDER values, confirm with the team
        if (day == 5 || day == 10)
        {
            quests.Add(new QuestEntity
            {
                GuildId = guild.Id,
                OwnerPlayerId = null, // shared, guild-wide
                Name = $"Combat de boss (jour {day})",
                DayAvailable = day,
                Type = "BossFight",
                Difficulty = level,
                DurationHours = 16,
                GoldReward = 500 + day * 50,
                MinimumLevelRequired = 1,
                MinimumTeamSize = 3,
                MaximumTeamSize = 5,
                RequiresAllRecruitCategories = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        return quests;
    }
}