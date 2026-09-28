using GuildManager.Logic.Gameplay.Characters;
using Xunit;

namespace GuildManager.Tests;

public class AdventurerTests
{
    [Fact]
    public void Heal_RestoresHealthPoints_CappedAtMax()
    {
        var warrior = new Warrior("Bram"); // MaxHealthPoints = 40
        warrior.Damage(30); // 40 -> 10

        warrior.Heal(15); // 10 + 15 = 25, well under the cap

        Assert.Equal(25, warrior.HealthPoints);
    }

    [Fact]
    public void Heal_NeverExceedsMaxHealthPoints()
    {
        var warrior = new Warrior("Bram");
        warrior.Damage(5); // 40 -> 35

        warrior.Heal(50); // would overshoot to 85 if uncapped

        Assert.Equal(40, warrior.HealthPoints);
    }

    [Fact]
    public void Heal_ClearsInjuredStatus()
    {
        var warrior = new Warrior("Bram");
        warrior.MarkInjured();
        Assert.True(warrior.IsInjured);

        warrior.Heal(10);

        Assert.False(warrior.IsInjured);
    }

    [Fact]
    public void Damage_NeverGoesBelowZero()
    {
        var warrior = new Warrior("Bram");

        warrior.Damage(1000);

        Assert.Equal(0, warrior.HealthPoints);
    }

    [Fact]
    public void MarkInjured_DropsHealthToThirtyPercentOfMax()
    {
        var warrior = new Warrior("Bram"); // Max = 40

        warrior.MarkInjured();

        Assert.True(warrior.IsInjured);
        Assert.Equal((int)(40 * 0.30), warrior.HealthPoints);
    }

    [Fact]
    public void GainExperience_BelowThreshold_DoesNotLevelUp()
    {
        var warrior = new Warrior("Bram");

        warrior.GainExperience(50); // threshold at level 1 is 100

        Assert.Equal(1, warrior.Level);
        Assert.Equal(50, warrior.Experience);
    }

    [Fact]
    public void GainExperience_AtThreshold_LevelsUpAndCarriesOverRemainder()
    {
        var warrior = new Warrior("Bram");

        warrior.GainExperience(120); // threshold at level 1 is 100

        Assert.Equal(2, warrior.Level);
        Assert.Equal(20, warrior.Experience); // 120 - 100 carried over
    }

    [Fact]
    public void GainExperience_CanTriggerMultipleLevelUpsAtOnce()
    {
        var warrior = new Warrior("Bram");

        // Level 1->2 costs 100, level 2->3 costs 200: 310 clears both.
        warrior.GainExperience(310);

        Assert.Equal(3, warrior.Level);
        Assert.Equal(10, warrior.Experience);
    }

    [Fact]
    public void CalculateSuccessRate_LevelMatchesQuest_AppliesBonus()
    {
        var warrior = new Warrior("Bram"); // Level 1, BaseSuccessRate 45

        int rateAtMatchingLevel = warrior.CalculateSuccessRate(questLevel: 1);
        int rateAtHigherLevel = warrior.CalculateSuccessRate(questLevel: 3);

        Assert.True(rateAtMatchingLevel > rateAtHigherLevel);
    }

    [Fact]
    public void CalculateSuccessRate_WhileInjured_TakesFifteenPointMalus()
    {
        var warrior = new Warrior("Bram");
        int rateBeforeInjury = warrior.CalculateSuccessRate(questLevel: 3); // no level-match bonus here

        warrior.MarkInjured();
        int rateWhileInjured = warrior.CalculateSuccessRate(questLevel: 3);

        Assert.Equal(rateBeforeInjury - 15, rateWhileInjured);
    }

    [Fact]
    public void Healer_CalculateSuccessRate_AlwaysReturnsZero()
    {
        var healer = new Healer("Elara");

        Assert.Equal(0, healer.CalculateSuccessRate(questLevel: 1));
        Assert.Equal(0, healer.CalculateSuccessRate(questLevel: 5));
    }

    [Fact]
    public void Healer_GroupMultiplier_IsOnePointThree()
    {
        var healer = new Healer("Elara");
        Assert.Equal(1.3, healer.GroupMultiplier);
    }

    [Fact]
    public void MarkUsedToday_ThenResetDailyUsage_RoundTrips()
    {
        var warrior = new Warrior("Bram");

        warrior.MarkUsedToday();
        Assert.True(warrior.UsedToday);

        warrior.ResetDailyUsage();
        Assert.False(warrior.UsedToday);
    }
}
