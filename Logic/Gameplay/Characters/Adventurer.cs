using System;

namespace GuildManager.Logic.Gameplay.Characters;

// Base class for every adventurer in the guild (generic or special).
public abstract class Adventurer
{
    public string Name { get; }
    public int HealthPoints { get; protected set; }

    // Health cap set at creation; healing never exceeds it.
    public int MaxHealthPoints { get; }

    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }

    // True once this recruit has already been sent on a quest today. A
    // recruit used in one quest cannot be reused in another the same day.
    public bool UsedToday { get; private set; }

    // True after coming back injured from a failed quest whose estimated
    // success rate was 30% or higher (below that, the team dies instead -
    // see Guild.AttemptQuest). An injured recruit takes a 15-point malus on
    // CalculateSuccessRate until healed with a potion (see Heal()).
    public bool IsInjured { get; private set; }

    // Base success rate: 35-45% for generic recruits, 80-85% for special
    // adventurers. The Healer overrides this mechanism entirely (see Healer.cs).
    protected int BaseSuccessRate { get; set; } = 40;

    protected Adventurer(string name, int healthPoints)
    {
        Name = name;
        HealthPoints = healthPoints;
        MaxHealthPoints = healthPoints;
    }

    // Restores HP to the recruit (e.g. a health potion), capped at
    // MaxHealthPoints, and cures the injured status if any.
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        HealthPoints = Math.Min(HealthPoints + amount, MaxHealthPoints);
        IsInjured = false;
    }

    // Deals damage to the recruit (e.g. a failed quest), floored at 0.
    public void Damage(int amount)
    {
        if (amount <= 0) return;
        HealthPoints = Math.Max(HealthPoints - amount, 0);
    }

    // Marks the recruit as injured after coming back from a failed quest,
    // and drops their HP to 30% of their max (visible on the roster until
    // healed with a potion - see Heal()).
    public void MarkInjured()
    {
        IsInjured = true;
        HealthPoints = (int)(MaxHealthPoints * 0.30);
    }

    // Grants experience and levels the recruit up as thresholds are crossed.
    public void GainExperience(int amount)
    {
        if (amount <= 0) return;

        Experience += amount;
        int nextLevelThreshold = Level * 100;

        while (Experience >= nextLevelThreshold)
        {
            Experience -= nextLevelThreshold;
            Level++;
            nextLevelThreshold = Level * 100;
        }
    }

    // Success rate for a quest of a given level (1-5). Level-parity bonus
    // (x1.5) as before; an injured recruit additionally takes a flat -15
    // point malus until healed.
    public virtual int CalculateSuccessRate(int questLevel)
    {
        double rate = BaseSuccessRate;
        if (Level == questLevel)
            rate *= 1.5;

        if (IsInjured)
            rate -= 15;

        return (int)Math.Clamp(Math.Round(rate), 5, 95);
    }

    // Reinjects a previously saved state (see SaveManager).
    public void RestoreProgress(int level, int experience, int healthPoints)
    {
        Level = level;
        Experience = experience;
        HealthPoints = healthPoints;
    }

    public void MarkUsedToday() => UsedToday = true;

    public void ResetDailyUsage() => UsedToday = false;

    public override string ToString() => $"{Name} (Lvl. {Level}, {HealthPoints} HP)";
}