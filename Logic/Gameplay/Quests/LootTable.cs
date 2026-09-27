using System;
using System.Collections.Generic;

namespace GuildManager.Logic.Gameplay.Quests;

// Random loot table, one pool of item names per QuestType. Drawn on a
// successful quest resolution (see Guild.AttemptQuest), on top of the
// quest's gold reward and the recruits' experience gain.
public static class LootTable
{
    private static readonly Random RandomGenerator = new();

    private static readonly Dictionary<QuestType, string[]> Pools = new()
    {
        [QuestType.Escort] = new[] { "Bourse de cuivre", "Dague émoussée", "Carte routière" },
        [QuestType.Exorcism] = new[] { "Fiole d'eau bénite", "Amulette fêlée", "Cendre purificatrice" },
        [QuestType.DungeonExploration] = new[] { "Gemme brute", "Parchemin ancien", "Clé rouillée" },
        [QuestType.SpecialAdventurer] = new[] { "Trophée personnel", "Souvenir gravé" },
        [QuestType.ImprobableNpc] = new[] { "Objet inexplicable", "Reçu illisible" },
        [QuestType.BossFight] = new[] { "Écaille de bête légendaire", "Fragment d'artefact", "Couronne ternie" },
    };

    // Draws one random loot item name for the given quest type. Returns
    // null if no pool is defined for that type (nothing dropped).
    public static string? DrawLoot(QuestType type)
    {
        if (!Pools.TryGetValue(type, out var items) || items.Length == 0)
            return null;

        return items[RandomGenerator.Next(items.Length)];
    }
}
