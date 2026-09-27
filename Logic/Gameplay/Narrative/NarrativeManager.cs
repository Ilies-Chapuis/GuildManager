using System;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.Logic.Gameplay.Narrative;

// Manages the active narrative voice and the permanent Serious -> Wtf switch.
// Serious is the default voice for the whole game; accepting an improbable
// NPC's quest is a point of no return, valid for the rest of the playthrough.
public sealed class NarrativeManager
{
    public NarrativeVoice CurrentVoice { get; private set; } = NarrativeVoice.Serious;
    public bool SwitchAlreadyTriggered { get; private set; }

    public event Action<ImprobableNpc>? OnSwitchToWtf;

    // Locks the game into the Wtf voice for good, the first time it is called.
    public void AcceptImprobableNpcQuest(ImprobableNpc npc)
    {
        if (SwitchAlreadyTriggered)
            return; // already on the Wtf route: nothing to redo

        CurrentVoice = NarrativeVoice.Wtf;
        SwitchAlreadyTriggered = true;
        OnSwitchToWtf?.Invoke(npc);
    }

    // Keeps the game on the Serious voice: refusing changes nothing.
    public void DeclineImprobableNpcQuest()
    {
        // The game continues normally in the Serious voice: nothing to change.
    }

    // Returns a dialogue entry's text for the currently active voice.
    public string GetText(DialogueEntry entry) => entry.GetText(CurrentVoice);

    // Returns a special adventurer's line for the given day, in the active voice.
    public string GetAdventurerText(SpecialAdventurer adventurer, int day) =>
        adventurer.Speak(CurrentVoice, day);

    // Reinjects a previously saved state (see SaveManager).
    public void RestoreState(NarrativeVoice voice, bool switchAlreadyTriggered)
    {
        CurrentVoice = voice;
        SwitchAlreadyTriggered = switchAlreadyTriggered;
    }
}
