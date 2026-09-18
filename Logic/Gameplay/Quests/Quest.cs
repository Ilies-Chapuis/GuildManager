using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.Logic.Gameplay.Quests;

// A quest offered to the guild. GDD 6.1: every quest has restrictions on
// which adventurers can be assigned; GDD 6.5: every quest has a duration in
// hours, charged against the day's 16-hour budget. Boss fights add a team
// size requirement and can demand one recruit of every generic category.
// FR : Une quête proposée à la guilde. Contraintes sur les aventuriers
// assignables, durée en heures. Les combats de boss ajoutent une exigence
// de taille d'équipe et peuvent exiger une recrue de chaque catégorie.
public sealed class Quest
{
    public string Name { get; }
    public QuestType Type { get; }
    public int Difficulty { get; }
    public int DurationHours { get; }
    public int GoldReward { get; }
    public int MinimumLevelRequired { get; }

    // Minimum number of recruits that must be assigned together (boss fights).
    // FR : Nombre minimum de recrues devant être assignées ensemble (combats de boss).
    public int MinimumTeamSize { get; }

    // If true, the team must contain at least one Warrior, one Healer and one Mage.
    // FR : Si vrai, l'équipe doit contenir au moins un Guerrier, un Healeur et un Mage.
    public bool RequiresAllRecruitCategories { get; }

    public bool IsResolved { get; private set; }
    public bool WasSuccessful { get; private set; }

    // Creates a quest with its type, difficulty, duration, reward, and
    // optional team-composition requirements (used by boss fights).
    // FR : Crée une quête avec son type, sa difficulté, sa durée, sa
    // récompense, et d'éventuelles exigences de composition d'équipe.
    public Quest(string name, QuestType type, int difficulty, int durationHours,
        int goldReward, int minimumLevelRequired = 1, int minimumTeamSize = 1,
        bool requiresAllRecruitCategories = false)
    {
        Name = name;
        Type = type;
        Difficulty = difficulty;
        DurationHours = durationHours;
        GoldReward = goldReward;
        MinimumLevelRequired = minimumLevelRequired;
        MinimumTeamSize = minimumTeamSize;
        RequiresAllRecruitCategories = requiresAllRecruitCategories;
    }

    // Records the outcome of the quest; can only be called once.
    // FR : Enregistre l'issue de la quête ; ne peut être appelé qu'une fois.
    public void MarkResolved(bool wasSuccessful)
    {
        IsResolved = true;
        WasSuccessful = wasSuccessful;
    }

    // Checks whether a single recruit meets the quest's minimum required level.
    // FR : Vérifie si une recrue atteint le niveau minimum requis par la quête.
    public bool IsAdventurerEligible(Adventurer recruit) => recruit.Level >= MinimumLevelRequired;

    // Checks the team-wide requirements: minimum size and, for boss fights,
    // that every generic recruit category (Warrior/Healer/Mage) is present.
    // FR : Vérifie les exigences de l'équipe : taille minimum et, pour les
    // combats de boss, la présence de chaque catégorie générique.
    public bool MeetsTeamRequirements(IReadOnlyList<Adventurer> team)
    {
        if (team.Count < MinimumTeamSize)
            return false;

        if (!RequiresAllRecruitCategories)
            return true;

        bool hasWarrior = team.OfType<Warrior>().Any();
        bool hasHealer = team.OfType<Healer>().Any();
        bool hasMage = team.OfType<Mage>().Any();

        return hasWarrior && hasHealer && hasMage;
    }

    // Returns a short human-readable summary of the quest.
    // FR : Renvoie un court résumé lisible de la quête.
    public override string ToString() =>
        $"{Name} [{Type}] - difficulty {Difficulty}, {DurationHours}h, {GoldReward} gold";
}
