using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GuildManager.Logic.Gameplay.Narrative;

// Loads the narrative content from Data/dialogues.json (at least 20 entries,
// each carrying both voices - see GDD 6.6/6.7). Characters call into this
// repository when they speak (see SpecialAdventurer.Speak).
public static class DialogueRepository
{
    private static List<DialogueEntry>? _cachedEntries;

    // Loads (and caches) every dialogue entry from the JSON file. Relative
    // paths are resolved against AppContext.BaseDirectory (the running
    // app's own folder), not the current working directory.
    public static IReadOnlyList<DialogueEntry> LoadEntries(string jsonPath = "Data/dialogues.json")
    {
        if (_cachedEntries is not null)
            return _cachedEntries;

        string fullPath = Path.IsPathRooted(jsonPath)
            ? jsonPath
            : Path.Combine(AppContext.BaseDirectory, jsonPath);

        string content = File.ReadAllText(fullPath);
        _cachedEntries = JsonSerializer.Deserialize<List<DialogueEntry>>(content)
                          ?? new List<DialogueEntry>();

        return _cachedEntries;
    }

    // Finds a dialogue entry by its unique id.
    public static DialogueEntry? Find(string id) =>
        LoadEntries().FirstOrDefault(e => e.Id == id);

    // Returns every dialogue entry scheduled for the given day.
    public static IEnumerable<DialogueEntry> ForDay(int day) =>
        LoadEntries().Where(e => e.Day == day);

    // Finds the line for a given character on a given day, if any.
    public static DialogueEntry? GetLineForCharacter(string character, int day) =>
        LoadEntries().FirstOrDefault(e => e.Character == character && e.Day == day);

    // Finds any line tagged with the given character's name, as a fallback.
    public static DialogueEntry? GetAnyLineForCharacter(string character) =>
        LoadEntries().FirstOrDefault(e => e.Character == character);
}
