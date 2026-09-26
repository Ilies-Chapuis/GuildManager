using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.Logic.MainMenu;

// Holds the single Guild instance for the current playthrough, so every
// UI view can reach the same game state instead of recreating one.
// FR : Garde l'unique instance de Guild de la partie en cours, pour que
// toutes les vues UI accèdent au même état plutôt que d'en recréer un.
public static class GameSessionManager
{
    public static Guild? Current { get; private set; }

    // Starts a fresh playthrough with the same day-1 starting roster used
    // in TerminalGameLoop (see Demo/TerminalGameLoop.cs line 161-167), so
    // the UI and the terminal harness begin from the same state.
    // FR : Démarre une nouvelle partie avec le même roster de départ que
    // TerminalGameLoop, pour que l'UI et le harnais terminal partent du
    // même état.
    public static void StartNewGame()
    {
        var guild = new Guild();
        guild.RecruitAdventurer(new Sameth());
        guild.RecruitAdventurer(new Meloap());
        guild.RecruitAdventurer(new Elowen());
        guild.RecruitAdventurer(new Warrior("Bram"));
        guild.RecruitAdventurer(new Healer("Elara"));
        guild.RecruitAdventurer(new Mage("Yssa"));

        Current = guild;
    }
}