using System;
using System.Collections.Generic;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Quests;

namespace GuildManager.Logic.MainMenu;

// Holds the single Guild instance for the current playthrough, so every
// UI view can reach the same game state instead of recreating one. Also
// owns the day's quest board and recruitment pool, so they persist as the
// player navigates between screens and shrink/refresh with the day cycle.
public static class GameSessionManager
{
    private static readonly Random Rng = new();
    private const int RecruitCandidateCount = 6;

    public static Guild? Current { get; private set; }

    // Whether the day-4 Black Market Donkey riddle has already been shown
    // this playthrough (it should only trigger once).
    public static bool DonkeyRiddleShown { get; set; }

    // Which days' improbable NPC encounter has already been shown (one NPC
    // per day, days 1-3: day 1 the Donkey, day 2 the Talking Cat, day 3 the
    // Messenger Pigeon). Each shows exactly once, on its own day.
    public static HashSet<int> ImprobableNpcDaysShown { get; } = new();

    // The quests currently on offer. A quest is removed from this list as
    // soon as it has been attempted (success or failure) - see Quest2View.
    public static List<Quest> TodaysQuests { get; private set; } = new();

    // The day's boss fight, if any (day 5: Nyxaria, day 10: Grendel). Null
    // on every other day. Set to null once attempted.
    public static Quest? TodaysBossQuest { get; set; }

    // Today's randomly generated recruitment candidates (not yet part of
    // the guild). Recruiting one removes it from this list.
    public static List<Adventurer> TodaysRecruitCandidates { get; private set; } = new();

    // Starts a fresh playthrough with the same day-1 starting roster used
    // in TerminalGameLoop (see Demo/TerminalGameLoop.cs line 161-167), so
    // the UI and the terminal harness begin from the same state.
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
        DonkeyRiddleShown = false;
        ImprobableNpcDaysShown.Clear();

        RegenerateDailyContent();
    }

    // Adopts a Guild reconstructed by SaveManager.Load as the current
    // session, and regenerates the day's quest board / recruitment pool
    // for it (these aren't part of the save file). One-time narrative
    // beats already past the loaded day are marked as shown, so reloading
    // mid-game doesn't replay them.
    public static void LoadGame(Guild guild)
    {
        Current = guild;

        int day = guild.Cycle.CurrentDay;
        DonkeyRiddleShown = day > 4;
        ImprobableNpcDaysShown.Clear();
        for (int d = 1; d <= 3; d++)
        {
            if (day > d) ImprobableNpcDaysShown.Add(d);
        }

        RegenerateDailyContent();
    }

    // Advances the day cycle: pays upkeep, clears the "already used today"
    // flag on every recruit, then regenerates the quest board and the
    // recruitment pool for the new day. Call this from the UI's "Jour
    // suivant" button.
    public static void AdvanceDay()
    {
        if (Current is null) return;

        Current.Cycle.AdvanceToNextDay();
        Current.ApplyDailyUpkeep();
        Current.ResetDailyUsage();

        RegenerateDailyContent();
    }

    private static void RegenerateDailyContent()
    {
        if (Current is null) return;

        TodaysQuests = QuestGenerator.GenerateForDay(Current.Cycle.CurrentDay, Current.Resources.Reputation);
        TodaysBossQuest = QuestGenerator.GenerateBossQuest(Current.Cycle.CurrentDay, Current.Resources.Reputation);

        var candidates = new List<Adventurer>();
        for (int i = 0; i < RecruitCandidateCount; i++)
        {
            string name = Current.NamePool.DrawUniqueName(Rng);
            candidates.Add(AdventurerFactory.GenerateRandomRecruit(name, Rng));
        }
        TodaysRecruitCandidates = candidates;
    }
}
