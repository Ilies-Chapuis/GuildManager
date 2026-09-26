using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;
using GuildManager.Logic.Gameplay.Quests;
using GuildManager.Logic.Gameplay.Resources;

namespace GuildManager.Logic.Gameplay;

// Aggregates the full state of a playthrough

public enum EndingType
{
    None,
    Good,
    Bad
}

public sealed class Guild
{
    public const int HpRestoredPerPotion = 10;

    // Below this estimated success rate, a failed quest kills the whole
    // team instead of merely injuring it.
    // FR : En dessous de ce taux de réussite estimé, une quête ratée tue
    // toute l'équipe au lieu de simplement la blesser.
    public const int DeathThreshold = 30;

    // "Debt collector" end condition (GDD): evaluated once day 10 is
    // reached, not a quest of its own.
    // FR : Condition de fin "récolteur de dette" : évaluée une fois le
    // jour 10 atteint, ce n'est pas une quête à part entière.
    private const int DebtDeadlineDay = 10;
    private const int DebtThreshold = 5000;

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
    // meets the quest's requirements, eligibility, the once-per-day usage
    // rule, and the day's hour budget (GDD 6.5). On failure, the outcome
    // depends on the estimated success rate that was actually sent: below
    // DeathThreshold, the whole team dies (removed from the roster);
    // otherwise the team survives but returns injured (a 15-point malus
    // applies to each of them until healed with a potion).
    // FR : Assigne un groupe de recrues à une quête. En cas d'échec, le
    // taux de réussite estimé au moment de l'envoi détermine la
    // conséquence : sous DeathThreshold, toute l'équipe meurt (retirée du
    // roster) ; sinon elle revient blessée (malus de 15 points par recrue
    // jusqu'à guérison par potion).
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

        int estimatedRate = QuestResolver.PreviewSuccessRate(quest, assignedRecruits, Resources.Food);
        bool wasSuccessful = QuestResolver.Resolve(quest, assignedRecruits, Resources.Food);

        if (wasSuccessful)
        {
            Resources.AddGold(quest.GoldReward);
            foreach (var recruit in assignedRecruits)
                recruit.MarkUsedToday();
        }
        else if (estimatedRate < DeathThreshold)
        {
            // The team dies - except special adventurers, who are immune
            // to death and merely come back injured instead.
            // FR : L'équipe meurt - sauf les personnages spéciaux, qui sont
            // insensibles à la mort et reviennent seulement blessés.
            foreach (var recruit in assignedRecruits)
            {
                if (recruit is SpecialAdventurer)
                {
                    recruit.MarkInjured();
                    recruit.MarkUsedToday();
                }
                else
                {
                    Roster.Remove(recruit);
                }
            }
        }
        else
        {
            // The team survives but comes back injured.
            // FR : L'équipe survit mais revient blessée.
            foreach (var recruit in assignedRecruits)
            {
                recruit.MarkInjured();
                recruit.MarkUsedToday();
            }
        }

        return wasSuccessful;
    }

    // Convenient overload to assign a single recruit to a quest.
    // FR : Surcharge pratique pour assigner une seule recrue à une quête.
    public bool AttemptQuest(Quest quest, Adventurer recruit) => AttemptQuest(quest, new[] { recruit });

    // Consumes a health potion to heal a recruit by 10 HP (and cure the
    // injured status); fails if the potion stock is empty.
    // FR : Consomme une potion de vie pour soigner une recrue de 10 PV
    // (et guérir le statut blessé) ; échoue si le stock est vide.
    public bool UseHealthPotion(Adventurer recruit)
    {
        if (!Resources.ConsumeHealthPotion())
            return false;

        recruit.Heal(HpRestoredPerPotion);
        return true;
    }

    // Evaluates the end-of-run "debt collector" condition once day 10 is
    // reached: the guild closes on a bad ending unless it holds at least
    // DebtThreshold gold. Standalone mechanic, not a Quest/QuestType.
    // NOTE: assumes DayCycle exposes CurrentDay and GuildResources exposes
    // Gold — adjust the two property names below if yours differ.
    // FR : Évalue la condition de fin "récolteur de dette" une fois le
    // jour 10 atteint : la guilde ferme sur une mauvaise fin sauf si elle
    // détient au moins DebtThreshold pièces d'or. Mécanisme autonome, pas
    // une Quest/QuestType.
    // NOTE : suppose que DayCycle expose CurrentDay et GuildResources
    // expose Gold — ajuste ces deux noms de propriété s'ils diffèrent
    // chez toi.
    public EndingType EvaluateEnding()
    {
        if (Cycle.CurrentDay < DebtDeadlineDay)
            return EndingType.None;

        return Resources.Gold >= DebtThreshold ? EndingType.Good : EndingType.Bad;
    }
}