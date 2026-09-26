using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.Logic.Gameplay.Quests;

// A quest offered to the guild. GDD 6.1: every quest has restrictions on
// which adventurers can be assigned; GDD 6.5: every quest has a duration in
// hours, charged against the day's 16-hour budget.
public sealed class Quest
{
    public string Name { get; }
    public int DayAvailable { get; }
    public QuestType Type { get; }
    public int Difficulty { get; }
    public int DurationHours { get; }
    public int GoldReward { get; }
    public int MinimumLevelRequired { get; }

    // Minimum/maximum number of recruits that can be assigned together.
    // FR : Nombre minimum/maximum de recrues assignables ensemble.
    public int MinimumTeamSize { get; }
    public int MaximumTeamSize { get; }

    // If true, the team must contain at least one Warrior, one Healer and one Mage (boss fights).
    // FR : Si vrai, l'équipe doit contenir les 3 catégories (combats de boss).
    public bool RequiresAllRecruitCategories { get; }

    // If set, the team must contain at least one recruit of this class
    // ("Warrior", "Healer" or "Mage"). Null means no such requirement.
    // FR : Si renseigné, l'équipe doit contenir au moins une recrue de cette classe.
    public string? RequiredClassName { get; }

    public bool IsResolved { get; private set; }
    public bool WasSuccessful { get; private set; }

    public Quest(string name, int dayAvailable, QuestType type, int difficulty, int durationHours,
        int goldReward, int minimumLevelRequired = 1, int minimumTeamSize = 1,
        int maximumTeamSize = 5, bool requiresAllRecruitCategories = false,
        string? requiredClassName = null)
    {
        Name = name;
        DayAvailable = dayAvailable;
        Type = type;
        Difficulty = difficulty;
        DurationHours = durationHours;
        GoldReward = goldReward;
        MinimumLevelRequired = minimumLevelRequired;
        MinimumTeamSize = minimumTeamSize;
        MaximumTeamSize = maximumTeamSize;
        RequiresAllRecruitCategories = requiresAllRecruitCategories;
        RequiredClassName = requiredClassName;
    }

    // Records the outcome of the quest; can only be called once during
    // normal gameplay flow.
    // FR : Enregistre l'issue de la quête ; ne peut être appelé qu'une fois
    // dans le déroulement normal du jeu.
    public void MarkResolved(bool wasSuccessful)
    {
        IsResolved = true;
        WasSuccessful = wasSuccessful;
    }

    // Restores a previously-resolved quest's state from a save file,
    // bypassing the normal resolution flow (no "already resolved" check,
    // no side effects). Meant to be called only by the persistence layer
    // right after loading a Quest from the database.
    // FR : Restaure l'état d'une quête déjà résolue depuis une sauvegarde,
    // sans passer par le flux de résolution normal (pas de vérification
    // "déjà résolue", pas d'effet de bord). Réservé à la couche
    // persistance, juste après le chargement d'une Quest depuis la BDD.
    public void RestoreResolution(bool isResolved, bool wasSuccessful)
    {
        IsResolved = isResolved;
        WasSuccessful = wasSuccessful;
    }

    // Checks whether a single recruit meets the quest's minimum required level.
    // FR : Vérifie si une recrue atteint le niveau minimum requis par la quête.
    public bool IsAdventurerEligible(Adventurer recruit) => recruit.Level >= MinimumLevelRequired;

    // Returns a human-readable reason the team cannot be assigned, or null if
    // every requirement (size, categories, required class) is satisfied.
    // FR : Renvoie la raison (lisible) pour laquelle l'équipe est refusée,
    // ou null si toutes les exigences sont respectées.
    public string? DescribeUnmetRequirement(IReadOnlyList<Adventurer> team)
    {
        if (team.Count < MinimumTeamSize)
            return $"This quest requires at least {MinimumTeamSize} recruit(s).";

        if (team.Count > MaximumTeamSize)
            return $"At most {MaximumTeamSize} recruits can be assigned to this quest.";

        if (RequiresAllRecruitCategories)
        {
            bool hasWarrior = team.OfType<Warrior>().Any();
            bool hasHealer = team.OfType<Healer>().Any();
            bool hasMage = team.OfType<Mage>().Any();
            if (!hasWarrior || !hasHealer || !hasMage)
                return "This quest requires at least one Warrior, one Healer and one Mage.";
        }

        if (RequiredClassName is not null && !HasClass(team, RequiredClassName))
            return $"This quest requires at least one {RequiredClassName}.";

        return null;
    }

    private static bool HasClass(IReadOnlyList<Adventurer> team, string className) => className switch
    {
        "Warrior" => team.OfType<Warrior>().Any(),
        "Healer" => team.OfType<Healer>().Any(),
        "Mage" => team.OfType<Mage>().Any(),
        _ => true
    };

    // Checks the team-wide requirements as a simple bool (see DescribeUnmetRequirement for details).
    // FR : Vérifie les exigences de l'équipe sous forme de booléen (voir DescribeUnmetRequirement pour le détail).
    public bool MeetsTeamRequirements(IReadOnlyList<Adventurer> team) => DescribeUnmetRequirement(team) is null;

    // Returns a short human-readable summary of the quest.
    // FR : Renvoie un court résumé lisible de la quête.
    public override string ToString() =>
        $"{Name} [{Type}] - difficulty {Difficulty}, {DurationHours}h, {GoldReward} gold";
}