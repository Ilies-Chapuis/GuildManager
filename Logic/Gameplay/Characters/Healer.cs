namespace GuildManager.Logic.Gameplay.Characters;

// Generic recruit - Healer class.
// Base look: white/gold robe, sacred symbol, healing orb.
// Support role: does not fight the quest itself (0% success rate alone) but
// multiplies the rest of the group's success rate by 1.2.
// FR : Recrue générique — classe Healeur. Rôle de soutien : 0% de réussite seul,
// multiplie par 1.2 le taux de réussite du reste du groupe.
public sealed class Healer : Adventurer, ISupportRecruit
{
    public double GroupMultiplier => 1.2;

    // Creates a Healer recruit with fixed base stats.
    // FR : Crée une recrue Healeur avec des statistiques de base fixes.
    public Healer(string name) : base(name, healthPoints: 28)
    {
        BaseSuccessRate = 50; // reference value, unused when alone (see below)
    }

    // Always returns 0: a lone healer does not fight the quest itself.
    // FR : Renvoie toujours 0 : un healeur seul ne combat pas la quête lui-même.
    public override int CalculateSuccessRate(int questDifficulty) => 0;
}
