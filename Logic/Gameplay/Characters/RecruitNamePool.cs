using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GuildManager.Logic.Gameplay.Characters;

// Draws unique recruit names from Data/recruit_names.json for the current
// playthrough: once a name has been drawn, it cannot be drawn again until a
// new Guild (new game) is created.
// FR : Tire des noms uniques depuis Data/recruit_names.json pour la partie
// en cours : un nom déjà tiré ne peut plus ressortir tant que la partie continue.
public sealed class RecruitNamePool
{
    private readonly List<string> _availableNames;
    private readonly HashSet<string> _usedNames = new();

    // Loads the full name list from JSON; nothing is marked used yet.
    // FR : Charge la liste complète des noms depuis le JSON ; rien n'est encore pris.
    public RecruitNamePool(string jsonPath = "Data/recruit_names.json")
    {
        string content = File.ReadAllText(jsonPath);
        _availableNames = JsonSerializer.Deserialize<List<string>>(content) ?? new List<string>();
    }

    public IReadOnlyCollection<string> UsedNames => _usedNames;

    // Draws a random name that hasn't been used yet this playthrough, and
    // marks it as used. Throws if every name has already been drawn.
    // FR : Tire un nom pas encore utilisé cette partie, et le marque comme pris.
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
    // FR : Réinjecte les noms déjà utilisés lors du chargement d'une sauvegarde.
    public void MarkNamesUsed(IEnumerable<string> names)
    {
        foreach (var name in names)
            _usedNames.Add(name);
    }
}