using System.Collections.Generic;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Quests;
using Xunit;

namespace GuildManager.Tests;

public class QuestTests
{
    [Fact]
    public void DescribeUnmetRequirement_TeamTooSmall_ReturnsReason()
    {
        var quest = new Quest("Escorte", 1, QuestType.Escort, difficulty: 1, durationHours: 4,
            goldReward: 100, minimumTeamSize: 2);
        var team = new List<Adventurer> { new Warrior("Bram") };

        string? reason = quest.DescribeUnmetRequirement(team);

        Assert.NotNull(reason);
    }

    [Fact]
    public void DescribeUnmetRequirement_TeamTooBig_ReturnsReason()
    {
        var quest = new Quest("Escorte", 1, QuestType.Escort, difficulty: 1, durationHours: 4,
            goldReward: 100, maximumTeamSize: 1);
        var team = new List<Adventurer> { new Warrior("Bram"), new Mage("Yssa") };

        string? reason = quest.DescribeUnmetRequirement(team);

        Assert.NotNull(reason);
    }

    [Fact]
    public void DescribeUnmetRequirement_MissingRequiredClass_ReturnsReason()
    {
        var quest = new Quest("Exorcisme", 1, QuestType.Exorcism, difficulty: 1, durationHours: 6,
            goldReward: 150, requiredClassName: "Healer");
        var team = new List<Adventurer> { new Warrior("Bram") };

        string? reason = quest.DescribeUnmetRequirement(team);

        Assert.NotNull(reason);
    }

    [Fact]
    public void DescribeUnmetRequirement_HasRequiredClass_ReturnsNull()
    {
        var quest = new Quest("Exorcisme", 1, QuestType.Exorcism, difficulty: 1, durationHours: 6,
            goldReward: 150, requiredClassName: "Healer");
        var team = new List<Adventurer> { new Healer("Elara") };

        Assert.Null(quest.DescribeUnmetRequirement(team));
    }

    [Fact]
    public void DescribeUnmetRequirement_NotEnoughSpecialAdventurers_ReturnsReason()
    {
        var bossQuest = new Quest("Nyxaria's Awakening", 5, QuestType.BossFight, difficulty: 5, durationHours: 10,
            goldReward: 1200, minimumTeamSize: 5, maximumTeamSize: 5, minimumSpecialAdventurers: 2);
        var team = new List<Adventurer>
        {
            new Sameth(), // only 1 special
            new Warrior("Bram"), new Healer("Elara"), new Mage("Yssa"), new Warrior("Second")
        };

        string? reason = bossQuest.DescribeUnmetRequirement(team);

        Assert.NotNull(reason);
    }

    [Fact]
    public void DescribeUnmetRequirement_EnoughSpecialAdventurers_ReturnsNull()
    {
        var bossQuest = new Quest("Nyxaria's Awakening", 5, QuestType.BossFight, difficulty: 5, durationHours: 10,
            goldReward: 1200, minimumTeamSize: 5, maximumTeamSize: 5, minimumSpecialAdventurers: 2);
        var team = new List<Adventurer>
        {
            new Sameth(), new Meloap(),
            new Warrior("Bram"), new Healer("Elara"), new Mage("Yssa")
        };

        Assert.Null(bossQuest.DescribeUnmetRequirement(team));
    }

    [Fact]
    public void IsAdventurerEligible_BelowMinimumLevel_ReturnsFalse()
    {
        var quest = new Quest("Exploration", 1, QuestType.DungeonExploration, difficulty: 1, durationHours: 8,
            goldReward: 300, minimumLevelRequired: 3);
        var recruit = new Warrior("Bram"); // starts at level 1

        Assert.False(quest.IsAdventurerEligible(recruit));
    }

    [Theory]
    [InlineData(4, 2)]
    [InlineData(6, 3)]
    [InlineData(8, 4)]
    public void FoodCost_IsHalfDurationInHours(int durationHours, int expectedFoodCost)
    {
        var quest = new Quest("Test", 1, QuestType.Escort, difficulty: 1, durationHours, goldReward: 100);

        Assert.Equal(expectedFoodCost, quest.FoodCost);
    }

    [Fact]
    public void MarkResolved_SetsIsResolvedAndOutcome()
    {
        var quest = new Quest("Test", 1, QuestType.Escort, difficulty: 1, durationHours: 4, goldReward: 100);

        quest.MarkResolved(wasSuccessful: true);

        Assert.True(quest.IsResolved);
        Assert.True(quest.WasSuccessful);
    }
}
