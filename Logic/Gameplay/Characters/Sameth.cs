namespace GuildManager.Logic.Gameplay.Characters;

// Sameth - repentant merchant-mage.
// Long-time friend of the player, revealed to have been possessed by Hedge
// during the Prologue (the Tartaros expedition), responsible for the attack
// on the old Guild. Once exorcised, he joins the new guild as a merchant and
// advisor. His actual lines live in Data/dialogues.json (see Speak()).
public sealed class Sameth : SpecialAdventurer
{
    public Sameth() : base(
        name: "Sameth",
        healthPoints: 30,
        backstory: "Repentant merchant-mage, once possessed by Hedge during the attack " +
                   "on the old Guild. Exorcised atop Tartaros, he joins the new guild as " +
                   "a merchant and advisor, haunted by guilt over what he did under the " +
                   "miasma's influence.")
    {
        BaseSuccessRate = 82; // special adventurer: 80-85%, one quest per day
    }
}