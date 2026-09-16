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

    // Base success rate, before adjustment for quest difficulty.
    // FR : Taux de réussite de base, avant ajustement par la difficulté de la quête.
    protected int BaseSuccessRate { get; set; } = 50;

    // Creates an adventurer with a name and a starting/maximum health value.
    // FR : Crée un aventurier avec un nom et des points de vie de départ/maximum.
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

    // Computes the success rate for a quest of a given difficulty.
    // FR : Calcule le taux de réussite pour une quête d'une difficulté donnée.
    public virtual int CalculateSuccessRate(int questDifficulty)
    {
        double rate = BaseSuccessRate + (Level - questDifficulty) * 1.3;
        return Math.Clamp((int)Math.Round(rate), 5, 95);
    }

    // Reinjects a previously saved state (see SaveManager).
    // FR : Réinjecte un état précédemment sauvegardé (voir SaveManager).
    public void RestoreProgress(int level, int experience, int healthPoints)
    {
        Level = level;
        Experience = experience;
        HealthPoints = healthPoints;
    }

    // Returns a short human-readable summary of the adventurer.
    // FR : Renvoie un court résumé lisible de l'aventurier.
    public override string ToString() => $"{Name} (Lvl. {Level}, {HealthPoints} HP)";
}
