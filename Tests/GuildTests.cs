using System.Collections.Generic;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Quests;
using Xunit;

namespace GuildManager.Tests;

public class GuildTests
{
    [Fact]
    public void RecruitAdventurer_AddsToRoster()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");

        guild.RecruitAdventurer(warrior);

        Assert.Contains(warrior, guild.Roster);
    }

    [Fact]
    public void AttemptQuest_TeamDoesNotMeetRequirements_ReturnsFalseAndChangesNothing()
    {
        var guild = new Guild();
        var quest = new Quest("Exorcisme", 1, QuestType.Exorcism, difficulty: 1, durationHours: 6,
            goldReward: 150, requiredClassName: "Healer");
        var warrior = new Warrior("Bram"); // no Healer in the team
        guild.RecruitAdventurer(warrior);

        int hoursBefore = guild.Cycle.RemainingHours;
        int goldBefore = guild.Resources.Gold;

        bool result = guild.AttemptQuest(quest, new List<Adventurer> { warrior });

        Assert.False(result);
        Assert.False(quest.IsResolved); // refused before any resolution attempt
        Assert.Equal(hoursBefore, guild.Cycle.RemainingHours); // no hours wasted
        Assert.Equal(goldBefore, guild.Resources.Gold);
    }

    [Fact]
    public void AttemptQuest_RecruitAlreadyUsedToday_ReturnsFalse()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        warrior.MarkUsedToday();
        guild.RecruitAdventurer(warrior);
        var quest = new Quest("Escorte", 1, QuestType.Escort, difficulty: 1, durationHours: 4, goldReward: 100);

        bool result = guild.AttemptQuest(quest, new List<Adventurer> { warrior });

        Assert.False(result);
        Assert.False(quest.IsResolved);
    }

    [Fact]
    public void AttemptQuest_NotEnoughHoursLeftToday_ReturnsFalse()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        guild.RecruitAdventurer(warrior);
        // Burn down almost all of the day's 16-hour budget first.
        guild.Cycle.ConsumeHours(15);
        var quest = new Quest("Exploration", 1, QuestType.DungeonExploration, difficulty: 1, durationHours: 8, goldReward: 300);

        bool result = guild.AttemptQuest(quest, new List<Adventurer> { warrior });

        Assert.False(result);
        Assert.Equal(1, guild.Cycle.RemainingHours); // untouched by the refused attempt
    }

    [Fact]
    public void AttemptQuest_NotEnoughFood_ReturnsFalse_AndDoesNotWasteHours()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        guild.RecruitAdventurer(warrior);
        guild.Resources.ConsumeFood(guild.Resources.Food); // drain the starting food stock to 0
        var quest = new Quest("Exploration", 1, QuestType.DungeonExploration, difficulty: 1, durationHours: 8, goldReward: 300); // FoodCost = 4

        int hoursBefore = guild.Cycle.RemainingHours;
        bool result = guild.AttemptQuest(quest, new List<Adventurer> { warrior });

        Assert.False(result);
        // The food check happens before hours are consumed, so a food
        // shortfall must never burn hours the team never actually used.
        Assert.Equal(hoursBefore, guild.Cycle.RemainingHours);
    }

    [Fact]
    public void UseHealthPotion_ConsumesOnePotion_HealsRecruit()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        warrior.Damage(30);
        int potionsBefore = guild.Resources.HealthPotions;

        bool result = guild.UseHealthPotion(warrior);

        Assert.True(result);
        Assert.Equal(potionsBefore - 1, guild.Resources.HealthPotions);
        Assert.Equal(25, warrior.HealthPoints); // 40 - 30 + 15
    }

    [Fact]
    public void UseHealthPotion_NoPotionsLeft_ReturnsFalse()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        for (int i = 0; i < 50; i++) guild.Resources.ConsumeHealthPotion(); // drain the stock

        bool result = guild.UseHealthPotion(warrior);

        Assert.False(result);
    }

    [Fact]
    public void EvaluateEnding_BeforeDayTen_ReturnsNone()
    {
        var guild = new Guild();

        Assert.Equal(EndingType.None, guild.EvaluateEnding());
    }

    [Fact]
    public void EvaluateEnding_AtDayTenWithEnoughGold_ReturnsGood()
    {
        var guild = new Guild();
        guild.Resources.AddGold(5000);
        for (int day = 1; day < 10; day++) guild.Cycle.AdvanceToNextDay();

        Assert.Equal(EndingType.Good, guild.EvaluateEnding());
    }

    [Fact]
    public void EvaluateEnding_AtDayTenWithoutEnoughGold_ReturnsBad()
    {
        var guild = new Guild();
        for (int day = 1; day < 10; day++) guild.Cycle.AdvanceToNextDay();

        Assert.Equal(EndingType.Bad, guild.EvaluateEnding());
    }

    [Fact]
    public void DescribeAttemptBlocker_TeamReady_ReturnsNull()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        guild.RecruitAdventurer(warrior);
        var quest = new Quest("Escorte", 1, QuestType.Escort, difficulty: 1, durationHours: 4, goldReward: 100);

        Assert.Null(guild.DescribeAttemptBlocker(quest, new List<Adventurer> { warrior }));
    }

    [Fact]
    public void DescribeAttemptBlocker_NotEnoughHours_MentionsDurationAndRemainingHours()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        guild.RecruitAdventurer(warrior);
        guild.Cycle.ConsumeHours(10); // 6h left today
        var quest = new Quest("Exploration", 1, QuestType.DungeonExploration, difficulty: 1, durationHours: 8, goldReward: 300);

        string? reason = guild.DescribeAttemptBlocker(quest, new List<Adventurer> { warrior });

        Assert.NotNull(reason);
        Assert.Contains("8h", reason!);
        Assert.Contains("6h", reason!);
    }

    [Fact]
    public void DescribeAttemptBlocker_RecruitAlreadyUsed_NamesTheRecruit()
    {
        var guild = new Guild();
        var warrior = new Warrior("Bram");
        warrior.MarkUsedToday();
        guild.RecruitAdventurer(warrior);
        var quest = new Quest("Escorte", 1, QuestType.Escort, difficulty: 1, durationHours: 4, goldReward: 100);

        string? reason = guild.DescribeAttemptBlocker(quest, new List<Adventurer> { warrior });

        Assert.NotNull(reason);
        Assert.Contains("Bram", reason!);
    }

    [Fact]
    public void AttemptQuest_WhenRefusedForLackOfHours_ChangesNothing()
    {
        // Regression: a refused attempt used to look like a failed quest in
        // the UI (no XP, no injuries, yet "quest failed" was displayed).
        var guild = new Guild();
        var healer = new Healer("Elara");
        var mage = new Mage("Yssa");
        guild.RecruitAdventurer(healer);
        guild.RecruitAdventurer(mage);
        guild.Cycle.ConsumeHours(12); // 4h left today
        var quest = new Quest("Exorcisme", 1, QuestType.Exorcism, difficulty: 1, durationHours: 6,
            goldReward: 150, requiredClassName: "Healer");
        int foodBefore = guild.Resources.Food;

        bool result = guild.AttemptQuest(quest, new List<Adventurer> { healer, mage });

        Assert.False(result);
        Assert.False(quest.IsResolved);
        Assert.Equal(foodBefore, guild.Resources.Food);
        Assert.Equal(4, guild.Cycle.RemainingHours);
        Assert.Equal(0, mage.Experience);
        Assert.False(mage.IsInjured);
    }
}
