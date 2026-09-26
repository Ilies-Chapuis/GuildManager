using GuildManager.Logic.Gameplay.Narrative;

namespace GuildManager.Logic.Gameplay.Characters;

// Special adventurer: fixed stats, backstory, and intervenes in at least one
// interactive dialogue (subject constraint). Content is looked up from the
// dialogue JSON (DialogueRepository) rather than hardcoded per class.
// Their high success rate (80-85%) is balanced by the same "one quest per
// day" rule that applies to every recruit (UsedToday, on Adventurer).
// FR : Aventurier spécial. Son taux de réussite élevé (80-85%) est
// compensé par la règle "une quête par jour" commune à toute recrue.
public abstract class SpecialAdventurer : Adventurer
{
    public string Backstory { get; }
    public bool UniqueEventTriggered { get; private set; }

    protected SpecialAdventurer(string name, int healthPoints, string backstory)
        : base(name, healthPoints)
    {
        Backstory = backstory;
    }

    // The character "speaks" by calling into the dialogue JSON for its line
    // on the given day, falling back to any line tagged with its name.
    // FR : Le personnage "parle" en interrogeant le JSON de dialogues pour
    // sa réplique de ce jour, avec repli sur n'importe quelle réplique portant son nom.
    public string Speak(NarrativeVoice voice, int day)
    {
        var entry = DialogueRepository.GetLineForCharacter(Name, day)
                    ?? DialogueRepository.GetAnyLineForCharacter(Name);

        return entry is null ? string.Empty : entry.GetText(voice);
    }

    public void MarkUniqueEventTriggered() => UniqueEventTriggered = true;

    // Reinjects a previously saved unique-event state (see SaveManager).
    // FR : Réinjecte un état d'événement unique précédemment sauvegardé (voir SaveManager).
    public void RestoreSpecialProgress(bool uniqueEventTriggered)
    {
        UniqueEventTriggered = uniqueEventTriggered;
    }
}