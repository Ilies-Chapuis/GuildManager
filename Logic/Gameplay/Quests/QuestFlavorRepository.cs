using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GuildManager.Logic.Gameplay.Quests;

// Loads generic quest flavor content from Data/quest_flavor.json (GDD-style
// companion to DialogueRepository, but keyed by QuestType instead of day).
// FR : Charge le contenu générique des quêtes depuis Data/quest_flavor.json.
public static class QuestFlavorRepository
{
    private static List<QuestFlavorEntry>? _cachedEntries;

    // Loads (and caches) every flavor entry from the JSON file.
    // FR : Charge (et met en cache) toutes les entrées depuis le JSON.
    public static IReadOnlyList<QuestFlavorEntry> LoadEntries(string jsonPath = "Data/quest_flavor.json")
    {
        if (_cachedEntries is not null)
            return _cachedEntries;

        string content = File.ReadAllText(jsonPath);
        _cachedEntries = JsonSerializer.Deserialize<List<QuestFlavorEntry>>(content)
                          ?? new List<QuestFlavorEntry>();

        return _cachedEntries;
    }

    // Picks a random flavor entry matching the given quest type, or null if none exist.
    // FR : Tire une entrée aléatoire correspondant au type de quête donné, ou null si aucune.
    public static QuestFlavorEntry? GetRandomForType(QuestType type, Random rng)
    {
        var matches = LoadEntries().Where(e => e.Type == type.ToString()).ToList();
        return matches.Count == 0 ? null : matches[rng.Next(matches.Count)];
    }
}