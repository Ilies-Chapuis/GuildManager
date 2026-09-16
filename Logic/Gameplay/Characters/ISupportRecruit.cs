namespace GuildManager.Logic.Gameplay.Characters;

// Marks a recruit who does not fight the quest directly, but multiplies the
// rest of the group's success rate (e.g. Healer). Alone, such a recruit
// contributes no chance of success at all - enforced by QuestResolver.
// FR : Marque une recrue qui ne combat pas directement mais multiplie le taux
// du reste du groupe (ex : Healer). Seule, elle n'apporte aucune chance de réussite.
public interface ISupportRecruit
{
    // Multiplier applied to the group's success rate when this recruit assists.
    // FR : Multiplicateur appliqué au taux de réussite du groupe quand cette recrue aide.
    double GroupMultiplier { get; }
}
