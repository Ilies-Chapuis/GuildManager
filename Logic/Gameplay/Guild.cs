using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;
using GuildManager.Logic.Gameplay.Quests;
using GuildManager.Logic.Gameplay.Resources;

namespace GuildManager.Logic.Gameplay;

// Aggregates the full state of a playthrough: roster, resources, day cycle,
// active narration, and the pool of unique recruit names. This is the root
// object the UI and API consult.
// FR : Agrège l'état complet d'une partie : roster, ressources, cycle de
// jour, narration active, et le pool de noms uniques. Objet racine.
public sealed class Guild
{
    public const int HpRestoredPerPotion = 10;

    public List<Adventurer> Roster { get; } = new();
    public GuildResources Resources { get; } = new();
    public DayCycle Cycle { get; } = new();
    public NarrativeManager Narration { get; } = new();
    public RecruitNamePool NamePool { get; } = new();

    // Adds a recruit to the guild's roster.
    // FR : Ajoute une recrue au roster de la guilde.
    public void RecruitAdventurer(Adventurer recruit) => Roster.Add(recruit);

    // Applies the day's food cost based on roster size (GDD 7.1).
    // FR : Applique le coût de nourriture du jour selon la taille du roster.
    public bool ApplyDailyUpkeep()
    {
        int cost = UpkeepCalculator.CalculateFoodCost(Roster.Count);
        return Resources.ConsumeFood(cost);
    }

    // Clears every recruit's daily usage flag. Call this once at the start
    // of each new day (e.g. right after Cycle.AdvanceToNextDay()).
    // FR : Réinitialise l'indicateur "déjà utilisé aujourd'hui" de chaque
    // recrue. À appeler une fois par nouveau jour.
    public void ResetDailyUsage()
    {
        foreach (var recruit in Roster)
            recruit.ResetDailyUsage();
    }

    // Assigns a group of recruits to a quest and resolves it if the team
    // meets the quest's requirements (size, categories), eligibility, the
    // once-per-day usage rule (no recruit can do two quests the same day -
    // the team must change, or new recruits must be hired), and the day's
    // hour budget (GDD 6.5).
    // FR : Assigne un groupe de recrues à une quête et la résout si l'équipe
    // respecte les exigences, l'éligibilité, la règle "une quête par jour"
    // (aucune recrue ne peut faire deux quêtes le même jour), et le budget
    // d'heures du jour.
    public bool AttemptQuest(Quest quest, IReadOnlyList<Adventurer> assignedRecruits)
    {
        if (!quest.MeetsTeamRequirements(assignedRecruits))
            return false; // team too small/large, or missing a required class

        if (assignedRecruits.Any(r => !quest.IsAdventurerEligible(r)))
            return false; // at least one recruit doesn't meet the required level

        if (assignedRecruits.Any(r => r.UsedToday))
            return false; // at least one recruit here already went on a quest today

        if (!Cycle.ConsumeHours(quest.DurationHours))
            return false; // not enough hours left today

        bool wasSuccessful = QuestResolver.Resolve(quest, assignedRecruits, Resources.Food);
        if (wasSuccessful)
            Resources.AddGold(quest.GoldReward);

        foreach (var recruit in assignedRecruits)
            recruit.MarkUsedToday();

        return wasSuccessful;
    }

    // Convenient overload to assign a single recruit to a quest.
    // FR : Surcharge pratique pour assigner une seule recrue à une quête.
    public bool AttemptQuest(Quest quest, Adventurer recruit) => AttemptQuest(quest, new[] { recruit });

    // Consumes a health potion to heal a recruit by 10 HP; fails if stock is empty.
    // FR : Consomme une potion de vie pour soigner une recrue de 10 PV ; échoue si le stock est vide.
    public bool UseHealthPotion(Adventurer recruit)
    {
        if (!Resources.ConsumeHealthPotion())
            return false;

        recruit.Heal(HpRestoredPerPotion);
        return true;
    }
}