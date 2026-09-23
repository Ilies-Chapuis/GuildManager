namespace GuildManager.Logic.Gameplay.Narrative;

// Improbable NPC - triggers the permanent switch to the Wtf voice.
// Accepting its quest is a point of no return.
// FR : PNJ improbable — déclenche la bascule définitive vers la Voix WTF.
// Accepter sa quête est un point de non-retour.
public sealed class ImprobableNpc
{
    public string Name { get; }
    public string Appearance { get; }
    public string QuestHook { get; }
    public int QuestDurationHours { get; }

    // Creates an improbable NPC with its name, appearance and quest hook.
    // FR : Crée un PNJ improbable avec son nom, son apparence et son accroche de quête.
    public ImprobableNpc(string name, string appearance, string questHook, int questDurationHours)
    {
        Name = name;
        Appearance = appearance;
        QuestHook = questHook;
        QuestDurationHours = questDurationHours;
    }

    // Predefined improbable NPC: the Black Market Donkey.
    // FR : PNJ improbable prédéfini : l'Âne du marché noir.
    public static ImprobableNpc BlackMarketDonkey => new(
        name: "Ane du marché noir",
        appearance: "A bipedal donkey in a worn trench coat, briefcase in hand, cracked monocle.",
        questHook: "Offers to trade resources for a favor that is nobody else's business.",
        questDurationHours: 3);

    // Predefined improbable NPC: the Self-Proclaimed Messenger Pigeon.
    // FR : PNJ improbable prédéfini : le Pigeon messager auto-proclamé.
    public static ImprobableNpc MessengerPigeon => new(
        name: "The Self-Proclaimed Messenger Pigeon",
        appearance: "A pigeon wearing a tiny postman's hat and a miniature satchel.",
        questHook: "Claims to carry an urgent royal message and demands an escort.",
        questDurationHours: 2);

    // Predefined improbable NPC: the Talking Cat.
    // FR : PNJ improbable prédéfini : le Chat qui parle.
    public static ImprobableNpc TalkingCat => new(
        name: "The Talking Cat (only at night)",
        appearance: "A feline silhouette in the shadows, sickly-sweet and sarcastic voice.",
        questHook: "Asks for help with a deliberately vague contract.",
        questDurationHours: 4);
}
