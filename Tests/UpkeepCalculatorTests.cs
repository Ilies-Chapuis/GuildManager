using GuildManager.Logic.Gameplay.Resources;
using Xunit;

namespace GuildManager.Tests;

public class UpkeepCalculatorTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(3, 3)]   // top of the "low" tier (x1)
    [InlineData(4, 8)]   // first day of the "moderate" tier (x2)
    [InlineData(6, 12)]  // top of the "moderate" tier
    [InlineData(7, 21)]  // first day of the "high" tier (x3)
    [InlineData(9, 27)]  // top of the "high" tier
    [InlineData(10, 50)] // first day of the "very high" tier (x5)
    public void CalculateFoodCost_MatchesExpectedTier(int recruitCount, int expectedCost)
    {
        int actual = UpkeepCalculator.CalculateFoodCost(recruitCount);
        Assert.Equal(expectedCost, actual);
    }

    [Fact]
    public void CalculateFoodCost_NegativeCount_ReturnsZero()
    {
        Assert.Equal(0, UpkeepCalculator.CalculateFoodCost(-5));
    }
}
