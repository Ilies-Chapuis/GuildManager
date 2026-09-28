namespace GuildManager.Logic.Dialogues;

/// <summary>
/// Une ligne/beat de dialogue. Correspond exactement au schema de
/// Demo/Data/dialogues.json (Id, Day, Character, SeriousText, WtfText),
/// avec deux champs optionnels en plus : Emotion et BoxSkin.
/// Les entrees existantes qui n'ont pas ces deux champs continuent
/// de fonctionner (Emotion -> "Neutral", BoxSkin -> "Default").
/// </summary>
public class DialogueEntry
{
    public string Id { get; set; } = "";
    public int Day { get; set; }
    public string Character { get; set; } = "";
    public string SeriousText { get; set; } = "";
    public string WtfText { get; set; } = "";

    /// <summary>
    /// Cle d'emotion declaree dans portraits.json pour ce personnage (ex: "Angry"),
    /// ou un index de case brut ("7"). Absent/null -> "Neutral".
    /// </summary>
    public string? Emotion { get; set; }

    /// <summary>
    /// Cle de skin de boite declaree dans portraits.json (ex: "Nature").
    /// Absent/null -> "Default".
    /// </summary>
    public string? BoxSkin { get; set; }

    /// <summary>Texte a afficher selon le mode actif (Serieux / WTF).</summary>
    public string GetText(bool wtfMode)
    {
        if (!wtfMode) return SeriousText;
        return string.IsNullOrEmpty(WtfText) ? SeriousText : WtfText;
    }
}
