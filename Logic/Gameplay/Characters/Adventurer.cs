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
    // recruit used in one quest cannot be reused in another the same day -
    // the team must change, or new recruits must be hired (per design).
    // FR : Vrai une fois cette recrue déjà envoyée en quête aujourd'hui.
    // Une même recrue ne peut pas faire deux quêtes le même jour.
    public bool UsedToday { get; private set; }

    // Base success rate: 35-45% for generic recruits, 80-85% for special
    // adventurers. It never changes with level - only the level-parity
    // bonus below reflects progression. The Healer overrides this
    // mechanism entirely: it has no success rate of its own (see Healer.cs).
    // FR : Taux de réussite de base : 35-45% pour une recrue générique,
    // 80-85% pour un personnage spécial. Le Healeur n'a pas de taux propre.
    protected int BaseSuccessRate { get; set; } = 40;

    protected Adventurer(string name, int healthPoints)
    {
        Name = name;
        HealthPoints = healthPoints;
        MaxHealthPoints = healthPoints;
    }

    // Restores HP to the recruit (e.g. a health potion), capped at MaxHealthPoints.
    // FR : Restaure des PV à la recrue (ex : potion de vie), plafonné au maximum.
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        HealthPoints = Math.Min(HealthPoints + amount, MaxHealthPoints);
    }

    // Deals damage to the recruit (e.g. a failed quest), floored at 0.
    // FR : Inflige des dégâts à la recrue (ex : quête ratée), plancher à 0.
    public void Damage(int amount)
    {
        if (amount <= 0) return;
        HealthPoints = Math.Max(HealthPoints - amount, 0);
    }

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

    // Success rate for a quest of a given level (1-5, rising every two days,
    // see TerminalGameLoop.GetQuestLevelForDay). When the recruit's own
    // Level matches the quest's level exactly, the base rate is multiplied
    // by 1.5 - encouraging the player to match recruits to fitting quests
    // as they level up, rather than always chasing the hardest quest.
    // FR : Taux de réussite pour une quête de niveau donné. Quand le niveau
    // de la recrue correspond exactement à celui de la quête, le taux de
    // base est multiplié par 1.5.
    public virtual int CalculateSuccessRate(int questLevel)
    {
        double rate = BaseSuccessRate;
        if (Level == questLevel)
            rate *= 1.5;

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

    // Marks this recruit as having gone on a quest today.
    // FR : Marque cette recrue comme déjà envoyée en quête aujourd'hui.
    public void MarkUsedToday() => UsedToday = true;

    // Clears the daily usage flag; called by Guild.ResetDailyUsage() at the
    // start of a new day.
    // FR : Réinitialise l'indicateur d'utilisation quotidienne, en début de journée.
    public void ResetDailyUsage() => UsedToday = false;

    public override string ToString() => $"{Name} (Lvl. {Level}, {HealthPoints} HP)";
}