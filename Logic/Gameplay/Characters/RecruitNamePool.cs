using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GuildManager.Logic.Gameplay.Characters;

// Draws unique recruit names from Data/recruit_names.json for the current
// playthrough: once a name has been drawn, it cannot be drawn again until a
// new Guild (new game) is created.
public sealed class RecruitNamePool
{
    private readonly List<string> _availableNames;
    private readonly HashSet<string> _usedNames = new();

    // Loads the full name list from JSON; nothing is marked used yet.
    // Relative paths are resolved against the running app's own directory
    // (AppContext.BaseDirectory), not the current working directory, so this
    // works the same whether launched via `dotnet run`, double-click on the
    // .exe, or from an IDE.
    public RecruitNamePool(string jsonPath = "Data/recruit_names.json")
    {
        string fullPath = Path.IsPathRooted(jsonPath)
            ? jsonPath
            : Path.Combine(AppContext.BaseDirectory, jsonPath);

        string content = File.ReadAllText(fullPath);
        _availableNames = JsonSerializer.Deserialize<List<string>>(content) ?? new List<string>();
    }

    public IReadOnlyCollection<string> UsedNames => _usedNames;

    // Draws a random name that hasn't been used yet this playthrough, and
    // marks it as used. Throws if every name has already been drawn.
    public string DrawUniqueName(Random rng)
    {
        var remaining = _availableNames.Where(n => !_usedNames.Contains(n)).ToList();
        if (remaining.Count == 0)
            throw new InvalidOperationException("No unused recruit names left in the pool.");

        string chosen = remaining[rng.Next(remaining.Count)];
        _usedNames.Add(chosen);
        return chosen;
    }

    // Reinjects previously used names when reloading a save (see SaveManager).
    public void MarkNamesUsed(IEnumerable<string> names)
    {
        foreach (var name in names)
            _usedNames.Add(name);
    }
}
