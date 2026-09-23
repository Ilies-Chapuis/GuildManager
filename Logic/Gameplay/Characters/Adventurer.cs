using System;

namespace GuildManager.Logic.Gameplay.Characters;

// Base class for every adventurer in the guild (generic or special).
// FR : Classe de base pour tout aventurier de la guilde (générique ou spécial).
public abstract class Adventurer
{
    public string Name { get; }
    public int HealthPoints { get; protected set; }

    // Health cap set at creation; healing never exceeds it.
    // FR : Plafond de PV fixé à la création ; le soin ne le dépasse jamais.
    public int MaxHealthPoints { get; }

    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }

    // True once this recruit has already been sent on a quest today. A
    // recruit used in one quest cannot be reused in another the same day.
    // FR : Vrai une fois cette recrue déjà envoyée en quête aujourd'hui.
    public bool UsedToday { get; private set; }

    // True after coming back injured from a failed quest whose estimated
    // success rate was 30% or higher (below that, the team dies instead -
    // see Guild.AttemptQuest). An injured recruit takes a 15-point malus on
    // CalculateSuccessRate until healed with a potion (see Heal()).
    // FR : Vrai après un retour blessé d'une quête ratée dont le taux
    // estimé était ≥ 30%. Applique un malus de 15 points tant que la
    // recrue n'est pas soignée par une potion.
    public bool IsInjured { get; private set; }

    // Base success rate: 35-45% for generic recruits, 80-85% for special
    // adventurers. The Healer overrides this mechanism entirely (see Healer.cs).
    // FR : Taux de réussite de base : 35-45% pour une recrue générique,
    // 80-85% pour un personnage spécial. Le Healeur n'a pas de taux propre.
    protected int BaseSuccessRate { get; set; } = 40;

    protected Adventurer(string name, int healthPoints)
    {
        Name = name;
        HealthPoints = healthPoints;
        MaxHealthPoints = healthPoints;
    }

    // Restores HP to the recruit (e.g. a health potion), capped at
    // MaxHealthPoints, and cures the injured status if any.
    // FR : Restaure des PV (ex : potion de vie), plafonné au maximum, et
    // guérit le statut "blessé" le cas échéant.
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        HealthPoints = Math.Min(HealthPoints + amount, MaxHealthPoints);
        IsInjured = false;
    }

    // Deals damage to the recruit (e.g. a failed quest), floored at 0.
    // FR : Inflige des dégâts à la recrue (ex : quête ratée), plancher à 0.
    public void Damage(int amount)
    {
        if (amount <= 0) return;
        HealthPoints = Math.Max(HealthPoints - amount, 0);
    }

    // Marks the recruit as injured after coming back from a failed quest.
    // FR : Marque la recrue comme blessée après un retour de quête ratée.
    public void MarkInjured() => IsInjured = true;

    // Grants experience and levels the recruit up as thresholds are crossed.
    // FR : Octroie de l'expérience et fait monter de niveau la recrue au fil des paliers.
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
    // FR : Taux de réussite pour une quête de niveau donné. Une recrue
    // blessée subit en plus un malus fixe de -15 points jusqu'à guérison.
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
    // FR : Réinjecte un état précédemment sauvegardé (voir SaveManager).
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