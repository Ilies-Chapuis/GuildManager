using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;

namespace GuildManager.Logic.Gameplay.SaveSystem;

// Local save/load for a playthrough (solo mode). Serializes the entire state
// of a Guild (day, resources, narrative voice, roster) into a JSON file, and
// can rebuild an identical Guild from that file.
// FR : Sauvegarde/chargement local d'une partie. Sérialise l'état complet
// d'une Guild en JSON, et sait reconstruire une Guild identique.
public static class SaveManager
{
    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    // Serializes the guild's full state to a JSON file at the given path.
    // FR : Sérialise l'état complet de la guilde dans un fichier JSON au chemin donné.
    public static void Save(Guild guild, string path = "Saves/save.json")
    {
        var data = new SaveData
        {
            Day = guild.Cycle.CurrentDay,
            RemainingHours = guild.Cycle.RemainingHours,
            Gold = guild.Resources.Gold,
            Food = guild.Resources.Food,
            HealthPotions = guild.Resources.HealthPotions,
            Reputation = guild.Resources.Reputation,
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
            }).ToList()
        };

        string? folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder))
            Directory.CreateDirectory(folder);

        File.WriteAllText(path, JsonSerializer.Serialize(data, WriteOptions));
    }

    // Rebuilds a full Guild instance from a previously saved JSON file.
    // FR : Reconstruit une instance complète de Guild depuis un fichier JSON sauvegardé.
    public static Guild Load(string path = "Saves/save.json")
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("No save file found.", path);

        var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path))
                   ?? throw new InvalidDataException("Invalid save file.");

        var guild = new Guild();

        guild.Cycle.RestoreState(data.Day, data.RemainingHours);
        guild.Resources.RestoreState(data.Gold, data.Food, data.HealthPotions, data.Reputation);

        var voice = Enum.Parse<NarrativeVoice>(data.CurrentVoice);
        guild.Narration.RestoreState(voice, data.SwitchAlreadyTriggered);

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
    // FR : Associe une instance concrète d'Adventurer à sa chaîne de type pour la sauvegarde.
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
