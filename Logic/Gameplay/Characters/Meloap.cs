namespace GuildManager.Logic.Gameplay.Characters;

// Meloap - lizardman adventurer.
// Unwavering ally of the player since their very first steps at the Guild of
// Fadriann (including the famous magic door from the Prologue). Becomes the
// guild's right hand. His lines live in Data/dialogues.json (see Speak()).
public sealed class Meloap : SpecialAdventurer
{
    public Meloap() : base(
        name: "Meloap",
        healthPoints: 50,
        backstory: "Lizardman adventurer, ally of the player since their very first " +
                   "steps at the Guild of Fadriann. Always ready to support recruits " +
                   "in the field.")
    {
        BaseSuccessRate = 85; // special adventurer: 80-85%, one quest per day
    }
}