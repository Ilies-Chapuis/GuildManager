namespace GuildManager.Logic.Gameplay.Characters;

// Elowen - high elf archer and ranger.
// New special recruit with no shared history with the player; her connection
// to nature lets her sense the miasma's first signs before anyone else.
// Her lines live in Data/dialogues.json (see Speak()).
// FR : Elowen — haute-elfe archère et rôdeuse, nouvelle recrue sans passé
// commun. Ses répliques vivent dans Data/dialogues.json.
public sealed class Elowen : SpecialAdventurer
{
    public Elowen() : base(
        name: "Elowen",
        healthPoints: 32,
        backstory: "High elf archer and ranger, joins the guild as a quiet sentinel " +
                   "able to detect the miasma's first signs in the vegetation.")
    {
        BaseSuccessRate = 83; // aventurier spécial : 80-85%, une seule quête par jour
    }
}