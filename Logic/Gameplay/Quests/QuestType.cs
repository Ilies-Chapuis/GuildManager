namespace GuildManager.Logic.Gameplay.Quests;

// Quest types (GDD 6.2). Once the Wtf voice is triggered, only quests from
// the Wtf pool are generated - filtering happens in the quest generator
// (local or API-side), not in this enum itself.
public enum QuestType
{
    Escort,
    Exorcism,
    DungeonExploration,
    SpecialAdventurer,
    ImprobableNpc,
    BossFight
}
