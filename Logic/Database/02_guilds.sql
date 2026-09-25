-- ============================================================
-- 2. GUILDS (guild shared between several players)
-- ============================================================

CREATE TABLE guilds (
    id                      SERIAL PRIMARY KEY,
    name                    VARCHAR(100) NOT NULL,

    -- Matches DayCycle.CurrentDay / RestoreState()
    current_day             INTEGER NOT NULL DEFAULT 1 CHECK (current_day BETWEEN 1 AND 10),

    -- Matches DayCycle.RemainingHours / RestoreState(). Reset to 16
    -- (DayCycle.HoursPerDay) whenever current_day is advanced.
    remaining_hours         INTEGER NOT NULL DEFAULT 16 CHECK (remaining_hours BETWEEN 0 AND 16),

    -- Shared resources (common pool), matches GuildResources
    gold                    INTEGER NOT NULL DEFAULT 0 CHECK (gold >= 0),
    food                    INTEGER NOT NULL DEFAULT 0 CHECK (food >= 0),
    health_potions          INTEGER NOT NULL DEFAULT 0 CHECK (health_potions >= 0),

    -- Result of Guild.EvaluateEnding() -- 'None' until day 10 is resolved
    ending                  VARCHAR(10) NOT NULL DEFAULT 'None'
        CHECK (ending IN ('None', 'Good', 'Bad')),

    created_at              TIMESTAMP NOT NULL DEFAULT NOW()
);