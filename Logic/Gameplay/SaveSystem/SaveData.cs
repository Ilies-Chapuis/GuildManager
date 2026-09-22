using System.Collections.Generic;

namespace GuildManager.Logic.Gameplay.SaveSystem;

// Exact shape of the JSON save file (solo mode - subject: "you must
// implement a local save/load system").
// FR : Forme exacte du fichier de sauvegarde JSON (mode solo).
public sealed class SaveData
{
    public int Day { get; set; } = 1;
    public int RemainingHours { get; set; } = 16;
    public int Gold { get; set; }
    public int Food { get; set; }
    public int HealthPotions { get; set; }
    public int Reputation { get; set; }
    public string CurrentVoice { get; set; } = "Serious";
    public bool SwitchAlreadyTriggered { get; set; }
    public List<RecruitSaveDto> Roster { get; set; } = new();

    // Names already drawn from RecruitNamePool this playthrough, so they
    // stay unavailable after reloading a save.
    // FR : Noms déjà tirés du RecruitNamePool cette partie, pour qu'ils
    // restent indisponibles après un chargement.
    public List<string> UsedRecruitNames { get; set; } = new();
}