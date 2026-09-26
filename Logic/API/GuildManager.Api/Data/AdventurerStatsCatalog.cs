namespace GuildManager.Api.Data;

// Mirrors the fixed base stats hardcoded in the team's C# classes
// (Warrior/Healer/Mage/Elowen/Meloap/Sameth). If those values change,
// update them here too.
public static class AdventurerStatsCatalog
{
    public static (int MaxHp, int BaseSuccessRate) GetBaseStats(string category, string? specialKey)
    {
        if (category == "Special")
        {
            return specialKey switch
            {
                "Elowen" => (32, 85),
                "Meloap" => (50, 88),
                "Sameth" => (30, 82),
                _ => throw new ArgumentException($"Unknown special key: {specialKey}")
            };
        }

        return category switch
        {
            "Warrior" => (40, 55),
            "Mage" => (22, 52),
            "Healer" => (28, 50),
            _ => throw new ArgumentException($"Unknown category: {category}")
        };
    }
}