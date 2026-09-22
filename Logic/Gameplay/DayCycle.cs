using System;

namespace GuildManager.Logic.Gameplay;

// Day/hour cycle for the guild (GDD 6.5 and section 2): a day is a shared
// 16-hour budget. Resolving a quest consumes hours; once the budget runs
// out, the day ends and play moves on to the next one.
// FR : Cycle jour/heures de la guilde : un budget partagé de 16h. Résoudre
// une quête consomme des heures ; une fois le budget épuisé, on change de jour.
public sealed class DayCycle
{
    public const int HoursPerDay = 16;
    public const int FinalDay = 10;

    public int CurrentDay { get; private set; } = 1;
    public int RemainingHours { get; private set; } = HoursPerDay;

    public bool IsDayOver => RemainingHours <= 0;
    public bool IsGameOver => CurrentDay > FinalDay;

    // Consumes a quest's hours if the remaining budget allows it.
    // FR : Consomme les heures d'une quête si le budget restant le permet.
    public bool ConsumeHours(int durationHours)
    {
        if (durationHours <= 0 || durationHours > RemainingHours)
            return false;

        RemainingHours -= durationHours;
        return true;
    }

    // Moves to the next day and resets the hour budget to 16.
    // FR : Passe au jour suivant et réinitialise le budget d'heures à 16.
    public void AdvanceToNextDay()
    {
        if (IsGameOver)
            throw new InvalidOperationException("The game is already over (past day 10).");

        CurrentDay++;
        RemainingHours = HoursPerDay;
    }

    // Reinjects a previously saved state (see SaveManager).
    // FR : Réinjecte un état précédemment sauvegardé (voir SaveManager).
    public void RestoreState(int day, int remainingHours)
    {
        CurrentDay = day;
        RemainingHours = remainingHours;
    }
}
