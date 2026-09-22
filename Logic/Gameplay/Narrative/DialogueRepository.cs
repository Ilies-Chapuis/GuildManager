using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GuildManager.Logic.Gameplay.Narrative;

// Loads the narrative content from Data/dialogues.json (at least 20 entries,
// each carrying both voices - see GDD 6.6/6.7). Characters call into this
// repository when they speak (see SpecialAdventurer.Speak).
// FR : Charge le contenu narratif depuis Data/dialogues.json (au moins 20
// entrées). Les personnages appellent ce dépôt lorsqu'ils parlent.
public static class DialogueRepository
{
    private static List<DialogueEntry>? _cachedEntries;

    // Loads (and caches) every dialogue entry from the JSON file.
    // FR : Charge (et met en cache) toutes les entrées de dialogue depuis le JSON.
    public static IReadOnlyList<DialogueEntry> LoadEntries(string jsonPath = "Data/dialogues.json")
    {
        if (_cachedEntries is not null)
            return _cachedEntries;

        string content = File.ReadAllText(jsonPath);
        _cachedEntries = JsonSerializer.Deserialize<List<DialogueEntry>>(content)
                          ?? new List<DialogueEntry>();

        return _cachedEntries;
    }

    // Finds a dialogue entry by its unique id.
    // FR : Trouve une entrée de dialogue par son identifiant unique.
    public static DialogueEntry? Find(string id) =>
        LoadEntries().FirstOrDefault(e => e.Id == id);

    // Returns every dialogue entry scheduled for the given day.
    // FR : Renvoie toutes les entrées de dialogue prévues pour ce jour.
    public static IEnumerable<DialogueEntry> ForDay(int day) =>
        LoadEntries().Where(e => e.Day == day);

    // Finds the line for a given character on a given day, if any.
    // FR : Trouve la réplique d'un personnage donné pour un jour donné, si elle existe.
    public static DialogueEntry? GetLineForCharacter(string character, int day) =>
        LoadEntries().FirstOrDefault(e => e.Character == character && e.Day == day);

    // Finds any line tagged with the given character's name, as a fallback.
    // FR : Trouve n'importe quelle réplique portant le nom de ce personnage, en repli.
    public static DialogueEntry? GetAnyLineForCharacter(string character) =>
        LoadEntries().FirstOrDefault(e => e.Character == character);
}
