using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GuildManager.Logic.Gameplay.Quests;

// Loads generic quest flavor content from Data/quest_flavor.json (GDD-style
// companion to DialogueRepository, but keyed by QuestType instead of day).
public static class QuestFlavorRepository
{
    private static List<QuestFlavorEntry>? _cachedEntries;

    // Loads (and caches) every flavor entry from the JSON file. Relative
    // paths are resolved against AppContext.BaseDirectory (the running
    // app's own folder), not the current working directory.
    public static IReadOnlyList<QuestFlavorEntry> LoadEntries(string jsonPath = "Data/quest_flavor.json")
    {
        if (_cachedEntries is not null)
            return _cachedEntries;

        string fullPath = Path.IsPathRooted(jsonPath)
            ? jsonPath
            : Path.Combine(AppContext.BaseDirectory, jsonPath);

        string content = File.ReadAllText(fullPath);
        _cachedEntries = JsonSerializer.Deserialize<List<QuestFlavorEntry>>(content)
                          ?? new List<QuestFlavorEntry>();

        return _cachedEntries;
    }

    // Picks a random flavor entry matching the given quest type, or null if none exist.
    public static QuestFlavorEntry? GetRandomForType(QuestType type, Random rng)
    {
        var matches = LoadEntries().Where(e => e.Type == type.ToString()).ToList();
        return matches.Count == 0 ? null : matches[rng.Next(matches.Count)];
    }
}
