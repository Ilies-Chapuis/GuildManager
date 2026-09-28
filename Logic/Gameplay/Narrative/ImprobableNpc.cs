namespace GuildManager.Logic.Gameplay.Narrative;

// Improbable NPC - triggers the permanent switch to the Wtf voice.
// Accepting its quest is a point of no return.
public sealed class ImprobableNpc
{
    public string Name { get; }
    public string Appearance { get; }
    public string QuestHook { get; }
    public int QuestDurationHours { get; }

    // Creates an improbable NPC with its name, appearance and quest hook.
    public ImprobableNpc(string name, string appearance, string questHook, int questDurationHours)
    {
        Name = name;
        Appearance = appearance;
        QuestHook = questHook;
        QuestDurationHours = questDurationHours;
    }

    // Predefined improbable NPC: the Black Market Donkey.
    public static ImprobableNpc BlackMarketDonkey => new(
        name: "Ane du marché noir",
        appearance: "Un âne bipède en trench-coat usé, porte-documents à la main, monocle fêlé.",
        questHook: "Propose d'échanger des ressources contre une faveur qui ne regarde personne d'autre.",
        questDurationHours: 3);

    // Predefined improbable NPC: the Self-Proclaimed Messenger Pigeon.
    public static ImprobableNpc MessengerPigeon => new(
        name: "Le pigeon voyageur autoproclamé",
        appearance: "Un pigeon portant une minuscule casquette de facteur et une sacoche miniature.",
        questHook: "Prétend porter un message royal urgent et exige une escorte.",
        questDurationHours: 2);

    // Predefined improbable NPC: the Talking Cat.
    public static ImprobableNpc TalkingCat => new(
        name: "Le Chat Potté trop badass",
        appearance: "Une silhouette féline dans l'ombre, une voix d'une douceur maladive et empreinte de sarcasme.",
        questHook: "Demande de l'aide concernant un contrat délibérément vague.",
        questDurationHours: 4);
}
