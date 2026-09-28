namespace GuildManager.Logic.Gameplay.Resources;

// Computes the daily food cost based on roster size (GDD 7.1): the cost per
// recruit rises in tiers rather than linearly.
public static class UpkeepCalculator
{
    // Returns the total daily food cost for a given number of recruits.
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
