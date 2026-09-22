namespace GuildManager.Logic.Gameplay.Resources;

// Guild resources. The three "items" the player manages are Gold, Food and
// Health Potions; Reputation is a separate stat (not consumable). Every
// consumable resource must be able to run out - removal methods return a
// bool instead of throwing.
// FR : Ressources de la guilde. Les trois objets sont Or, Nourriture et
// Potions de vie ; la Réputation reste une statistique à part.
public sealed class GuildResources
{
    public int Gold { get; private set; }
    public int Reputation { get; private set; }
    public int Food { get; private set; }
    public int HealthPotions { get; private set; }

    // Creates the guild's resource pool with starting amounts.
    // FR : Crée le pool de ressources de la guilde avec des montants de départ.
    public GuildResources(int startingGold = 500, int startingFood = 20, int startingHealthPotions = 10)
    {
        Gold = startingGold;
        Food = startingFood;
        HealthPotions = startingHealthPotions;
    }

    // Adds gold to the treasury.
    // FR : Ajoute de l'or au trésor.
    public void AddGold(int amount)
    {
        if (amount > 0) Gold += amount;
    }

    // Spends gold if enough is available; returns false otherwise.
    // FR : Dépense de l'or si le montant disponible suffit ; sinon renvoie false.
    public bool SpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount) return false;
        Gold -= amount;
        return true;
    }

    // Adds reputation points to the guild.
    // FR : Ajoute des points de réputation à la guilde.
    public void AddReputation(int amount) => Reputation += amount;

    // Consumes food if enough is available; returns false otherwise.
    // FR : Consomme de la nourriture si le stock suffit ; sinon renvoie false.
    public bool ConsumeFood(int quantity)
    {
        if (quantity <= 0 || Food < quantity) return false;
        Food -= quantity;
        return true;
    }

    // Adds food to the guild's stock.
    // FR : Ajoute de la nourriture au stock de la guilde.
    public void AddFood(int quantity)
    {
        if (quantity > 0) Food += quantity;
    }

    // Consumes a health potion if the stock allows it; returns false otherwise.
    // FR : Consomme une potion de vie si le stock le permet ; sinon renvoie false.
    public bool ConsumeHealthPotion(int quantity = 1)
    {
        if (quantity <= 0 || HealthPotions < quantity) return false;
        HealthPotions -= quantity;
        return true;
    }

    // Adds health potions to the guild's stock.
    // FR : Ajoute des potions de vie au stock de la guilde.
    public void AddHealthPotions(int quantity)
    {
        if (quantity > 0) HealthPotions += quantity;
    }

    // Checks the day-10 win condition (GDD 8.1): at least 5000 gold.
    // FR : Vérifie la condition de victoire du jour 10 : au moins 5000 or.
    public bool ReachedFinalGoal() => Gold >= 5000;

    // Reinjects a previously saved state (see SaveManager).
    // FR : Réinjecte un état précédemment sauvegardé (voir SaveManager).
    public void RestoreState(int gold, int food, int healthPotions, int reputation)
    {
        Gold = gold;
        Food = food;
        HealthPotions = healthPotions;
        Reputation = reputation;
    }
}
