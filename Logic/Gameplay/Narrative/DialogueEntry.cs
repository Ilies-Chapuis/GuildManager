namespace GuildManager.Logic.Gameplay.Narrative;

// A single dialogue entry, carrying both of its voice versions.
// Every key scene exists in BOTH the Serious and the Wtf voice.
// FR : Une entrée de dialogue portant ses deux versions. Chaque scène-clé
// existe à la fois en Voix Sérieuse et en Voix WTF.
public sealed class DialogueEntry
{
    public string Id { get; set; } = string.Empty;
    public int Day { get; set; }
    public string Character { get; set; } = string.Empty;
    public string SeriousText { get; set; } = string.Empty;
    public string WtfText { get; set; } = string.Empty;

    // Returns the text matching the requested voice.
    // FR : Renvoie le texte correspondant à la voix demandée.
    public string GetText(NarrativeVoice voice) =>
        voice == NarrativeVoice.Serious ? SeriousText : WtfText;
}
