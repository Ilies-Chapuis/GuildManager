namespace GuildManager.Logic.Gameplay.Characters;

// Elowen - high elf archer and ranger.
// New special recruit with no shared history with the player; her connection
// to nature lets her sense the miasma's first signs before anyone else.
// Her lines live in Data/dialogues.json (see Speak()).
// FR : Elowen — haute-elfe archère et rôdeuse, nouvelle recrue sans passé commun.
// Ses répliques vivent dans Data/dialogues.json.
public sealed class Elowen : SpecialAdventurer
{
    // Creates Elowen with her fixed lore, stats and backstory.
    // FR : Crée Elowen avec son lore, ses stats et son histoire fixes.
    public Elowen() : base(
        name: "Elowen",
        healthPoints: 32,
        backstory: "High elf archer and ranger, joins the guild as a quiet sentinel " +
                   "able to detect the miasma's first signs in the vegetation.")
    {
        BaseSuccessRate = 85; // special adventurer: high reliability (> 80%)
    }
}
