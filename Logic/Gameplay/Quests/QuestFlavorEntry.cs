namespace GuildManager.Logic.Gameplay.Quests;

// Generic flavor content for a non-boss quest type: a short objective
// summary plus an intro line in each narrative voice. Several entries can
// share the same Type for variety - one is picked at random when the quest
// is launched.
public sealed class QuestFlavorEntry
{
    public string Type { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string SeriousIntro { get; set; } = string.Empty;
    public string WtfIntro { get; set; } = string.Empty;
}