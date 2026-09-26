namespace GuildManager.Logic.Gameplay.Quests;

// Generic flavor content for a non-boss quest type: a short objective
// summary plus an intro line in each narrative voice. Several entries can
// share the same Type for variety - one is picked at random when the quest
// is launched.
// FR : Contenu générique pour un type de quête non-boss : un résumé
// d'objectif et une réplique d'introduction par voix. Plusieurs entrées
// peuvent partager le même Type pour varier ; une est tirée au hasard.
public sealed class QuestFlavorEntry
{
    public string Type { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string SeriousIntro { get; set; } = string.Empty;
    public string WtfIntro { get; set; } = string.Empty;
}