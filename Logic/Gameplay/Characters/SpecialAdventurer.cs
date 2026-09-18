using GuildManager.Logic.Gameplay.Narrative;

namespace GuildManager.Logic.Gameplay.Characters;

// Special adventurer: fixed stats, backstory, and intervenes in at least one
// interactive dialogue (subject constraint). Content is looked up from the
// dialogue JSON (DialogueRepository) rather than hardcoded per class.
// FR : Aventurier spécial : stats fixes, histoire, intervient dans au moins un
// dialogue interactif. Le contenu vient du JSON de dialogues, pas du code.
public abstract class SpecialAdventurer : Adventurer
{
    public string Backstory { get; }
    public bool UniqueEventTriggered { get; private set; }

    // Creates a special adventurer with a name, health, and fixed backstory.
    // FR : Crée un aventurier spécial avec un nom, des PV et une histoire fixe.
    protected SpecialAdventurer(string name, int healthPoints, string backstory)
        : base(name, healthPoints)
    {
        Backstory = backstory;
    }

    // Looks up this character's line for the given day and voice in the
    // dialogue JSON, falling back to any line tagged with its name.
    // FR : Récupère la réplique de ce personnage pour ce jour et cette voix
    // dans le JSON de dialogues, avec repli sur n'importe quelle réplique portant son nom.
    public string Speak(NarrativeVoice voice, int day)
    {
        var entry = DialogueRepository.GetLineForCharacter(Name, day)
                    ?? DialogueRepository.GetAnyLineForCharacter(Name);

        return entry is null ? string.Empty : entry.GetText(voice);
    }

    // Flags that this character's unique story event has already happened.
    // FR : Marque que l'événement unique de ce personnage s'est déjà produit.
    public void MarkUniqueEventTriggered() => UniqueEventTriggered = true;

    // Reinjects a previously saved unique-event state (see SaveManager).
    // FR : Réinjecte un état d'événement unique précédemment sauvegardé (voir SaveManager).
    public void RestoreSpecialProgress(bool uniqueEventTriggered)
    {
        UniqueEventTriggered = uniqueEventTriggered;
    }
}
