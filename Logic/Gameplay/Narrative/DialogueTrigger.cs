using System;

namespace GuildManager.Logic.Gameplay.Narrative;

// Trigger condition for an interactive dialogue: "dialogues must be
// triggered according to conditions you define" (subject constraint).
// FR : Condition de déclenchement d'un dialogue interactif, selon des
// conditions fixées par le développeur (contrainte du sujet).
public sealed class DialogueTrigger
{
    public string DialogueEntryId { get; }
    public int MinimumDay { get; }
    public Func<int, bool>? AdditionalCondition { get; }

    // Creates a trigger tied to a dialogue entry, a minimum day, and an
    // optional extra condition.
    // FR : Crée un déclencheur lié à une entrée de dialogue, un jour minimum,
    // et une condition supplémentaire optionnelle.
    public DialogueTrigger(string dialogueEntryId, int minimumDay, Func<int, bool>? additionalCondition = null)
    {
        DialogueEntryId = dialogueEntryId;
        MinimumDay = minimumDay;
        AdditionalCondition = additionalCondition;
    }

    // Checks whether this trigger can fire on the given day.
    // FR : Vérifie si ce déclencheur peut s'activer au jour donné.
    public bool CanTrigger(int currentDay) =>
        currentDay >= MinimumDay && (AdditionalCondition?.Invoke(currentDay) ?? true);
}
