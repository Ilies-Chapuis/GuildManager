using System;
using System.Collections.Generic;

namespace GuildManager.Logic.Gameplay.Quests;

// Generates the quests available for a given day. Quest level scales with
// the day (1 on days 1-3, then +1 every two days, up to 5 on day 10),
// matching Adventurer.Level's own scale. Escort requires a Warrior,
// Exorcism requires a Healer. Extracted here (rather than staying inline in
// the TMP terminal harness) so the future WPF UI can call it directly.
// FR : Génère les quêtes disponibles pour un jour donné. Extrait ici (plutôt
// que de rester dans le harnais terminal TMP) pour que l'UI puisse
// l'appeler directement.
public static class QuestGenerator
{
    // Quest level for a given day: level 1 on days 1-3, then +1 every two
    // days, reaching level 5 on day 10.
    // FR : Niveau de quête pour un jour donné : 1 les jours 1-3, puis +1
    // tous les 2 jours, jusqu'à 5 au jour 10.
    public static int GetQuestLevelForDay(int day)
    {
        if (day <= 3) return 1;
        return 1 + (int)Math.Ceiling((day - 3) / 2.0);
    }

    // Generates the standard set of non-boss quests offered on a given day.
    // FR : Génère l'ensemble standard de quêtes non-boss proposées un jour donné.
    public static List<Quest> GenerateForDay(int day)
    {
        int level = GetQuestLevelForDay(day);
        return new List<Quest>
        {
            new($"Escorte (jour {day})", QuestType.Escort,
                difficulty: level, durationHours: 4, goldReward: 80 + day * 10,
                requiredClassName: "Warrior"),
            new($"Exorcisme (jour {day})", QuestType.Exorcism,
                difficulty: level, durationHours: 6, goldReward: 120 + day * 15,
                requiredClassName: "Healer"),
            new($"Exploration de donjon (jour {day})", QuestType.DungeonExploration,
                difficulty: level, durationHours: 8, goldReward: 150 + day * 20),
        };
    }
}