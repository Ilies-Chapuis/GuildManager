using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;

namespace GuildManager.Logic.Gameplay.SaveSystem;

// Local save/load for a playthrough (solo mode). Serializes the entire state
// of a Guild (day, resources, narrative voice, roster, used recruit names)
// into a JSON file, and can rebuild an identical Guild from that file.
public static class SaveManager
{
    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    // Resolves a relative save path against the running app's own directory
    // (AppContext.BaseDirectory), not the current working directory - same
    // fix as RecruitNamePool/DialogueRepository, so saving works the same
    // whether launched via dotnet run, double-click, or from an IDE.
    private static string ResolvePath(string path) =>
        Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);

    // Serializes the guild's full state to a JSON file at the given path.
    public static void Save(Guild guild, string path = "Saves/save.json")
    {
        path = ResolvePath(path);

        var data = new SaveData
        {
            Day = guild.Cycle.CurrentDay,
            RemainingHours = guild.Cycle.RemainingHours,
            Gold = guild.Resources.Gold,
            Food = guild.Resources.Food,
            HealthPotions = guild.Resources.HealthPotions,
            Reputation = guild.Resources.Reputation,
            Inventory = guild.Resources.Inventory.ToList(),
            CurrentVoice = guild.Narration.CurrentVoice.ToString(),
            SwitchAlreadyTriggered = guild.Narration.SwitchAlreadyTriggered,
            Roster = guild.Roster.Select(r => new RecruitSaveDto
            {
                Name = r.Name,
                Type = DetermineType(r),
                Level = r.Level,
                Experience = r.Experience,
                HealthPoints = r.HealthPoints,
                UniqueEventTriggered = r is SpecialAdventurer special && special.UniqueEventTriggered
            }).ToList(),
            UsedRecruitNames = guild.NamePool.UsedNames.ToList()
        };

        string? folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder))
            Directory.CreateDirectory(folder);

        File.WriteAllText(path, JsonSerializer.Serialize(data, WriteOptions));
    }

    // Rebuilds a full Guild instance from a previously saved JSON file.
    public static Guild Load(string path = "Saves/save.json")
    {
        path = ResolvePath(path);

        if (!File.Exists(path))
            throw new FileNotFoundException("No save file found.", path);

        var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path))
                   ?? throw new InvalidDataException("Invalid save file.");

        var guild = new Guild();

        guild.Cycle.RestoreState(data.Day, data.RemainingHours);
        guild.Resources.RestoreState(data.Gold, data.Food, data.HealthPotions, data.Reputation, data.Inventory);

        var voice = Enum.Parse<NarrativeVoice>(data.CurrentVoice);
        guild.Narration.RestoreState(voice, data.SwitchAlreadyTriggered);
        guild.NamePool.MarkNamesUsed(data.UsedRecruitNames);

        foreach (var savedRecruit in data.Roster)
        {
            var recruit = AdventurerFactory.CreateRecruit(savedRecruit.Type, savedRecruit.Name);
            recruit.RestoreProgress(savedRecruit.Level, savedRecruit.Experience, savedRecruit.HealthPoints);

            if (recruit is SpecialAdventurer special)
                special.RestoreSpecialProgress(savedRecruit.UniqueEventTriggered);

            guild.RecruitAdventurer(recruit);
        }

        return guild;
    }

    // Maps a concrete Adventurer instance back to its save-file type string.
    private static string DetermineType(Adventurer recruit) => recruit switch
    {
        Warrior => "Warrior",
        Healer => "Healer",
        Mage => "Mage",
        Sameth => "Sameth",
        Meloap => "Meloap",
        Elowen => "Elowen",
        _ => throw new InvalidOperationException($"Recruit type not supported by save system: {recruit.GetType().Name}")
    };
}
