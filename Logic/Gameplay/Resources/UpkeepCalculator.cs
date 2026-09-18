namespace GuildManager.Logic.Gameplay.Resources;

// Computes the daily food cost based on roster size (GDD 7.1): the cost per
// recruit rises in tiers rather than linearly.
// FR : Calcule le coût de nourriture quotidien selon la taille du roster,
// par paliers plutôt que linéairement.
public static class UpkeepCalculator
{
    // Returns the total daily food cost for a given number of recruits.
    // FR : Renvoie le coût de nourriture quotidien total pour un nombre de recrues donné.
    public static int CalculateFoodCost(int recruitCount)
    {
        return recruitCount switch
        {
            <= 0 => 0,
            <= 3 => recruitCount * 1,  // low
            <= 6 => recruitCount * 2,  // moderate
            <= 9 => recruitCount * 3,  // high
            _ => recruitCount * 5      // very high - oversized roster
        };
    }
}
