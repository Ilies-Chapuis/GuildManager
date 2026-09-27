using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GuildManager.Logic.Dialogues;

/// <summary>Charge et expose les lignes de dialogues.json.</summary>
public class DialogueService
{
    private readonly List<DialogueEntry> _entries;

    public DialogueService(string dialoguesJsonPath)
    {
        var json = File.ReadAllText(dialoguesJsonPath);
        _entries = JsonSerializer.Deserialize<List<DialogueEntry>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DialogueEntry>();
    }

    public IReadOnlyList<DialogueEntry> All => _entries;

    public DialogueEntry? GetById(string id) =>
        _entries.FirstOrDefault(e => e.Id == id);

    /// <summary>Toutes les lignes d'un jour donne, dans l'ordre du fichier.</summary>
    public IReadOnlyList<DialogueEntry> GetByDay(int day) =>
        _entries.Where(e => e.Day == day).ToList();
}
