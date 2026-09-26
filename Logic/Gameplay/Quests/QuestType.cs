namespace GuildManager.Logic.Gameplay.Quests;

// Quest types (GDD 6.2). Once the Wtf voice is triggered, only quests from
// the Wtf pool are generated - filtering happens in the quest generator
// (local or API-side), not in this enum itself.
// FR : Types de quête. Une fois la Voix WTF déclenchée, seules les quêtes du
// pool WTF sont générées — le filtrage se fait dans le générateur, pas ici.
public enum QuestType
{
    Escort,
    Exorcism,
    DungeonExploration,
    SpecialAdventurer,
    ImprobableNpc,
    BossFight
}
