using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuildManager.Logic.Gameplay.Characters;

// JSON template for a generic recruit (Data/generic_recruits.json).
// IsSupport/GroupMultiplier are mostly informative here: the actual support
// behaviour still lives in code via ISupportRecruit (Healer).
public sealed class RecruitTemplateDto
{
    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("HealthPoints")]
    public int HealthPoints { get; set; }

    [JsonPropertyName("BaseSuccessRate")]
    public int BaseSuccessRate { get; set; }

    [JsonPropertyName("IsSupport")]
    public bool IsSupport { get; set; }

    [JsonPropertyName("GroupMultiplier")]
    public double GroupMultiplier { get; set; } = 1.0;
}

// Loads generic recruit templates from JSON and instantiates the right C#
// class (Warrior/Healer/Mage) with a given name. Also knows how to recreate
// special adventurers by type, used when reloading a save (SaveManager).
public static class AdventurerFactory
{
    private static List<RecruitTemplateDto>? _cachedTemplates;

    // Creates the concrete Adventurer instance matching the given type name.
    public static Adventurer CreateRecruit(string type, string name)
    {
        return type switch
        {
            "Warrior" => new Warrior(name),
            "Healer" => new Healer(name),
            "Mage" => new Mage(name),
            "Sameth" => new Sameth(),
            "Meloap" => new Meloap(),
            "Elowen" => new Elowen(),
            _ => throw new ArgumentException($"Unknown recruit type: {type}")
        };
    }

    // Loads (and caches) the recruit templates from a JSON file. Relative
    // paths are resolved against AppContext.BaseDirectory (the running
    // app's own folder), not the current working directory.
    public static IReadOnlyList<RecruitTemplateDto> LoadTemplates(string jsonPath = "Data/generic_recruits.json")
    {
        if (_cachedTemplates is not null)
            return _cachedTemplates;

        string fullPath = Path.IsPathRooted(jsonPath)
            ? jsonPath
            : Path.Combine(AppContext.BaseDirectory, jsonPath);

        string content = File.ReadAllText(fullPath);
        _cachedTemplates = JsonSerializer.Deserialize<List<RecruitTemplateDto>>(content)
                            ?? new List<RecruitTemplateDto>();

        return _cachedTemplates;
    }

    // Generates a random recruit among the types available in the JSON.
    public static Adventurer GenerateRandomRecruit(string name, Random rng, string jsonPath = "Data/generic_recruits.json")
    {
        var templates = LoadTemplates(jsonPath);
        var chosen = templates[rng.Next(templates.Count)];
        return CreateRecruit(chosen.Type, name);
    }
}
