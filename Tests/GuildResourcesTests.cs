using System.Collections.Generic;
using GuildManager.Logic.Gameplay.Resources;
using Xunit;

namespace GuildManager.Tests;

public class GuildResourcesTests
{
    [Fact]
    public void NewResources_StartWithGivenAmounts()
    {
        var resources = new GuildResources(startingGold: 500, startingFood: 20, startingHealthPotions: 10);

        Assert.Equal(500, resources.Gold);
        Assert.Equal(20, resources.Food);
        Assert.Equal(10, resources.HealthPotions);
        Assert.Equal(0, resources.Reputation);
        Assert.Empty(resources.Inventory);
    }

    [Fact]
    public void SpendGold_EnoughAvailable_SucceedsAndDeducts()
    {
        var resources = new GuildResources(startingGold: 100);

        bool result = resources.SpendGold(60);

        Assert.True(result);
        Assert.Equal(40, resources.Gold);
    }

    [Fact]
    public void SpendGold_NotEnoughAvailable_FailsAndLeavesGoldUnchanged()
    {
        var resources = new GuildResources(startingGold: 50);

        bool result = resources.SpendGold(100);

        Assert.False(result);
        Assert.Equal(50, resources.Gold);
    }

    [Fact]
    public void SpendGold_NegativeOrZeroAmount_Fails()
    {
        var resources = new GuildResources(startingGold: 100);

        Assert.False(resources.SpendGold(0));
        Assert.False(resources.SpendGold(-10));
    }

    [Fact]
    public void ConsumeFood_ExactAmountAvailable_Succeeds()
    {
        var resources = new GuildResources(startingFood: 10);

        bool result = resources.ConsumeFood(10);

        Assert.True(result);
        Assert.Equal(0, resources.Food);
    }

    [Fact]
    public void ConsumeFood_MoreThanAvailable_FailsAndLeavesStockUnchanged()
    {
        var resources = new GuildResources(startingFood: 5);

        bool result = resources.ConsumeFood(6);

        Assert.False(result);
        Assert.Equal(5, resources.Food);
    }

    [Fact]
    public void AddReputation_CanGoNegative()
    {
        var resources = new GuildResources();

        resources.AddReputation(-30);

        Assert.Equal(-30, resources.Reputation);
    }

    [Fact]
    public void ConsumeHealthPotion_DefaultQuantityOne_DecrementsStock()
    {
        var resources = new GuildResources(startingHealthPotions: 3);

        bool result = resources.ConsumeHealthPotion();

        Assert.True(result);
        Assert.Equal(2, resources.HealthPotions);
    }

    [Fact]
    public void ConsumeHealthPotion_EmptyStock_Fails()
    {
        var resources = new GuildResources(startingHealthPotions: 0);

        Assert.False(resources.ConsumeHealthPotion());
    }

    [Fact]
    public void AddLoot_AppendsItemToInventory()
    {
        var resources = new GuildResources();

        resources.AddLoot("Bourse de cuivre");
        resources.AddLoot("Gemme brute");

        Assert.Equal(new[] { "Bourse de cuivre", "Gemme brute" }, resources.Inventory);
    }

    [Fact]
    public void AddLoot_IgnoresNullOrWhitespace()
    {
        var resources = new GuildResources();

        resources.AddLoot("");
        resources.AddLoot("   ");

        Assert.Empty(resources.Inventory);
    }

    [Theory]
    [InlineData(4999, false)]
    [InlineData(5000, true)]
    [InlineData(6000, true)]
    public void ReachedFinalGoal_ChecksFiveThousandGoldThreshold(int gold, bool expected)
    {
        var resources = new GuildResources(startingGold: gold);

        Assert.Equal(expected, resources.ReachedFinalGoal());
    }

    [Fact]
    public void RestoreState_ReinjectsEverySavedValue()
    {
        var resources = new GuildResources();

        resources.RestoreState(gold: 250, food: 8, healthPotions: 2, reputation: -15, inventory: new List<string> { "Clé rouillée" });

        Assert.Equal(250, resources.Gold);
        Assert.Equal(8, resources.Food);
        Assert.Equal(2, resources.HealthPotions);
        Assert.Equal(-15, resources.Reputation);
        Assert.Equal(new[] { "Clé rouillée" }, resources.Inventory);
    }
}
