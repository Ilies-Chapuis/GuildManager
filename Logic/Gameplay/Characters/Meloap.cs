namespace GuildManager.Logic.Gameplay.Characters;

// Meloap - lizardman adventurer.
// Unwavering ally of the player since their very first steps at the Guild of
// Fadriann (including the famous magic door from the Prologue). Becomes the
// guild's right hand. His lines live in Data/dialogues.json (see Speak()).
// FR : Meloap — aventurier homme-lézard, allié indéfectible depuis les débuts.
// Ses répliques vivent dans Data/dialogues.json.
public sealed class Meloap : SpecialAdventurer
{
    // Creates Meloap with his fixed lore, stats and backstory.
    // FR : Crée Meloap avec son lore, ses stats et son histoire fixes.
    public Meloap() : base(
        name: "Meloap",
        healthPoints: 50,
        backstory: "Lizardman adventurer, ally of the player since their very first " +
                   "steps at the Guild of Fadriann. Always ready to support recruits " +
                   "in the field.")
    {
        BaseSuccessRate = 88; // special adventurer: the sturdiest of the roster (> 80%)
    }
}
