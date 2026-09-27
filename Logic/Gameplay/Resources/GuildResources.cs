using System.Collections.Generic;

namespace GuildManager.Logic.Gameplay.Resources;

// Guild resources. The three "items" the player manages are Gold, Food and
// Health Potions; Reputation is a separate stat (not consumable), affected
// by quest outcomes and used by QuestGenerator to scale future rewards.
// Inventory holds the loot items collected from successful quests.
public sealed class GuildResources
{
    public int Gold { get; private set; }
    public int Reputation { get; private set; }
    public int Food { get; private set; }
    public int HealthPotions { get; private set; }
    public List<string> Inventory { get; private set; } = new();

    // Creates the guild's resource pool with starting amounts.
    public GuildResources(int startingGold = 500, int startingFood = 20, int startingHealthPotions = 10)
    {
        Gold = startingGold;
        Food = startingFood;
        HealthPotions = startingHealthPotions;
    }

    // Adds gold to the treasury.
    public void AddGold(int amount)
    {
        if (amount > 0) Gold += amount;
    }

    // Spends gold if enough is available; returns false otherwise.
    public bool SpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount) return false;
        Gold -= amount;
        return true;
    }

    // Adds reputation points to the guild. Can be negative (a failed quest
    // costs reputation - see Guild.AttemptQuest).
    public void AddReputation(int amount) => Reputation += amount;

    // Consumes food if enough is available; returns false otherwise.
    public bool ConsumeFood(int quantity)
    {
        if (quantity <= 0 || Food < quantity) return false;
        Food -= quantity;
        return true;
    }

    // Adds food to the guild's stock.
    public void AddFood(int quantity)
    {
        if (quantity > 0) Food += quantity;
    }

    // Consumes a health potion if the stock allows it; returns false otherwise.
    public bool ConsumeHealthPotion(int quantity = 1)
    {
        if (quantity <= 0 || HealthPotions < quantity) return false;
        HealthPotions -= quantity;
        return true;
    }

    // Adds health potions to the guild's stock.
    public void AddHealthPotions(int quantity)
    {
        if (quantity > 0) HealthPotions += quantity;
    }

    // Adds a loot item to the guild's inventory (see LootTable).
    public void AddLoot(string itemName)
    {
        if (!string.IsNullOrWhiteSpace(itemName))
            Inventory.Add(itemName);
    }

    // Checks the day-10 win condition (GDD 8.1): at least 5000 gold.
    public bool ReachedFinalGoal() => Gold >= 5000;

    // Reinjects a previously saved state (see SaveManager).
    public void RestoreState(int gold, int food, int healthPotions, int reputation, List<string>? inventory = null)
    {
        Gold = gold;
        Food = food;
        HealthPotions = healthPotions;
        Reputation = reputation;
        Inventory = inventory ?? new List<string>();
    }
}
