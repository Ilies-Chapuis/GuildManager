using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuildManager.Logic.Gameplay.Characters;

// JSON template for a generic recruit (Data/generic_recruits.json).
// IsSupport/GroupMultiplier are mostly informative here: the actual support
// behaviour still lives in code via ISupportRecruit (Healer).
// FR : Gabarit JSON d'une recrue générique. IsSupport/GroupMultiplier sont
// surtout informatifs : le vrai comportement reste dans ISupportRecruit.
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
// FR : Charge les gabarits de recrues génériques depuis le JSON et instancie
// la bonne classe C#. Sait aussi recréer les personnages spéciaux par type.
public static class AdventurerFactory
{
    private static List<RecruitTemplateDto>? _cachedTemplates;

    // Creates the concrete Adventurer instance matching the given type name.
    // FR : Crée l'instance concrète d'Adventurer correspondant au type donné.
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

    // Loads (and caches) the recruit templates from a JSON file.
    // FR : Charge (et met en cache) les gabarits de recrues depuis un fichier JSON.
    public static IReadOnlyList<RecruitTemplateDto> LoadTemplates(string jsonPath = "Data/generic_recruits.json")
    {
        if (_cachedTemplates is not null)
            return _cachedTemplates;

        string content = File.ReadAllText(jsonPath);
        _cachedTemplates = JsonSerializer.Deserialize<List<RecruitTemplateDto>>(content)
                            ?? new List<RecruitTemplateDto>();

        return _cachedTemplates;
    }

    // Generates a random recruit among the types available in the JSON.
    // FR : Génère une recrue aléatoire parmi les types disponibles dans le JSON.
    public static Adventurer GenerateRandomRecruit(string name, Random rng, string jsonPath = "Data/generic_recruits.json")
    {
        var templates = LoadTemplates(jsonPath);
        var chosen = templates[rng.Next(templates.Count)];
        return CreateRecruit(chosen.Type, name);
    }
}
