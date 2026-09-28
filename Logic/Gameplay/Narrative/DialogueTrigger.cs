using System;

namespace GuildManager.Logic.Gameplay.Narrative;

// Trigger condition for an interactive dialogue: "dialogues must be
// triggered according to conditions you define" (subject constraint).
public sealed class DialogueTrigger
{
    public string DialogueEntryId { get; }
    public int MinimumDay { get; }
    public Func<int, bool>? AdditionalCondition { get; }

    // Creates a trigger tied to a dialogue entry, a minimum day, and an
    // optional extra condition.
    public DialogueTrigger(string dialogueEntryId, int minimumDay, Func<int, bool>? additionalCondition = null)
    {
        DialogueEntryId = dialogueEntryId;
        MinimumDay = minimumDay;
        AdditionalCondition = additionalCondition;
    }

    // Checks whether this trigger can fire on the given day.
    public bool CanTrigger(int currentDay) =>
        currentDay >= MinimumDay && (AdditionalCondition?.Invoke(currentDay) ?? true);
}
