using System;
using System.Collections.Generic;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Quests;
using Xunit;

namespace GuildManager.Tests;

public class QuestResolverTests
{
    private static Quest MakeQuest(QuestType type = QuestType.Escort, int difficulty = 1, int durationHours = 4) =>
        new("Test Quest", dayAvailable: 1, type, difficulty, durationHours, goldReward: 100);

    [Fact]
    public void PreviewSuccessRate_TeamOfOnlySupports_ReturnsZero()
    {
        var quest = MakeQuest();
        var team = new List<Adventurer> { new Healer("Elara") };

        int rate = QuestResolver.PreviewSuccessRate(quest, team, currentFood: 15);

        Assert.Equal(0, rate);
    }

    [Fact]
    public void PreviewSuccessRate_GenericRecruitAlone_TakesSoloPenalty()
    {
        var quest = MakeQuest(difficulty: 1);
        var soloTeam = new List<Adventurer> { new Warrior("Bram") };
        var pairedTeam = new List<Adventurer> { new Warrior("Bram"), new Warrior("Second") };

        int soloRate = QuestResolver.PreviewSuccessRate(quest, soloTeam, currentFood: 15);
        int pairedRate = QuestResolver.PreviewSuccessRate(quest, pairedTeam, currentFood: 15);

        // Going solo should always be worse than the same recruit backed up,
        // since the pair gets both the team bonus AND avoids the solo penalty.
        Assert.True(soloRate < pairedRate);
    }

    [Fact]
    public void PreviewSuccessRate_SpecialAdventurerAlone_DoesNotTakeSoloPenalty()
    {
        var quest = MakeQuest(difficulty: 1); // Sameth starts at level 1 -> matches, triggers the x1.5 bonus
        var team = new List<Adventurer> { new Sameth() };

        int rate = QuestResolver.PreviewSuccessRate(quest, team, currentFood: 15);

        // Sameth (BaseSuccessRate 82) at a level-matching difficulty already
        // clamps to 95 inside CalculateSuccessRate before the solo penalty
        // would even apply. If the "no penalty for specials" exemption were
        // broken, this would come back at 95-15=80 instead.
        Assert.Equal(95, rate);
    }

    [Fact]
    public void PreviewSuccessRate_HealerInGroup_MultipliesRestOfTeamRate()
    {
        var quest = MakeQuest(difficulty: 1);
        var team = new List<Adventurer> { new Warrior("Bram"), new Healer("Elara") };

        int actualRate = QuestResolver.PreviewSuccessRate(quest, team, currentFood: 15); // 15 = neutral food (no modifier)

        // Recompute the formula by hand, in the same order QuestResolver
        // applies it: (average fighter rate + team bonus) * support
        // multiplier, then clamp. Food=15 sits in the "no modifier" band
        // (between the debuff and bonus thresholds), so it contributes 0.
        var bram = new Warrior("Bram");
        double baseRate = bram.CalculateSuccessRate(quest.Difficulty); // fighters average = Bram alone
        double withTeamBonus = baseRate + 8.0; // team of 2 -> +8 once
        double withHealerMultiplier = withTeamBonus * 1.3; // Healer.GroupMultiplier
        int expected = (int)Math.Clamp(Math.Round(withHealerMultiplier), 5, 95);

        Assert.Equal(expected, actualRate);
    }

    [Fact]
    public void PreviewSuccessRate_BossFight_AppliesExtraPenalty()
    {
        var normalQuest = MakeQuest(QuestType.DungeonExploration, difficulty: 1);
        var bossQuest = MakeQuest(QuestType.BossFight, difficulty: 1);
        // Kept small on purpose: a big/optimal team pushes the pre-clamp
        // rate so high that both sides saturate at 95%, hiding the penalty.
        var team = new List<Adventurer> { new Sameth(), new Warrior("Bram") };

        int normalRate = QuestResolver.PreviewSuccessRate(normalQuest, team, currentFood: 15);
        int bossRate = QuestResolver.PreviewSuccessRate(bossQuest, team, currentFood: 15);

        Assert.True(bossRate < normalRate);
    }

    [Theory]
    [InlineData(25, 10)]  // above the bonus threshold (20): +10
    [InlineData(15, 0)]   // between thresholds: no modifier
    [InlineData(5, -10)]  // below the debuff threshold (7), but not empty: -10
    [InlineData(0, -25)]  // empty stock: the harsher critical debuff
    public void PreviewSuccessRate_FoodStock_AppliesExpectedModifier(int food, int expectedDelta)
    {
        var quest = MakeQuest(difficulty: 1);
        var team = new List<Adventurer> { new Warrior("Bram"), new Warrior("Second") };

        int rateAtNeutralFood = QuestResolver.PreviewSuccessRate(quest, team, currentFood: 15);
        int rateAtGivenFood = QuestResolver.PreviewSuccessRate(quest, team, currentFood: food);

        // Both are clamped to [5, 95], so only assert the direction/size of
        // the shift when neither side is pinned against a clamp boundary.
        if (rateAtNeutralFood is > 5 and < 95 && rateAtGivenFood is > 5 and < 95)
            Assert.Equal(rateAtNeutralFood + expectedDelta, rateAtGivenFood);
    }

    [Fact]
    public void PreviewSuccessRate_NeverGoesBelowFivePercentOrAboveNinetyFive()
    {
        var quest = MakeQuest(QuestType.BossFight, difficulty: 5);
        var weakTeam = new List<Adventurer> { new Mage("Yssa") };

        int rate = QuestResolver.PreviewSuccessRate(quest, weakTeam, currentFood: 0);

        Assert.InRange(rate, 5, 95);
    }

    [Fact]
    public void Resolve_EmptyTeam_Throws()
    {
        var quest = MakeQuest();

        Assert.Throws<ArgumentException>(() =>
            QuestResolver.Resolve(quest, new List<Adventurer>(), currentFood: 15));
    }

    [Fact]
    public void Resolve_AlreadyResolvedQuest_Throws()
    {
        var quest = MakeQuest();
        var team = new List<Adventurer> { new Sameth() };
        QuestResolver.Resolve(quest, team, currentFood: 15);

        Assert.Throws<InvalidOperationException>(() =>
            QuestResolver.Resolve(quest, team, currentFood: 15));
    }

    [Fact]
    public void Resolve_MarksQuestResolvedAndGrantsExperience()
    {
        var quest = MakeQuest(difficulty: 3);
        var recruit = new Sameth();
        int experienceBefore = recruit.Experience;

        QuestResolver.Resolve(quest, new List<Adventurer> { recruit }, currentFood: 15);

        Assert.True(quest.IsResolved);
        // Some XP was granted either way (full difficulty on success, half on failure).
        Assert.True(recruit.Experience > experienceBefore || recruit.Level > 1);
    }

    [Fact]
    public void Resolve_TeamOfOnlySupports_FailsWithoutRolling_AndStillGrantsConsolationXp()
    {
        var quest = MakeQuest(difficulty: 4);
        var healer = new Healer("Elara");

        bool result = QuestResolver.Resolve(quest, new List<Adventurer> { healer }, currentFood: 15);

        Assert.False(result);
        Assert.True(quest.IsResolved);
        Assert.False(quest.WasSuccessful);
        Assert.True(healer.Experience > 0); // consolation XP = quest.ExperienceReward / 2
    }
}
