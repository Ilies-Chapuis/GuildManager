-- ============================================================
-- 2. GUILDS (guild shared between several players)
-- ============================================================

-- Matches Guild.EvaluateEnding() in the C# code:
-- None while day < 10, then Good if Gold >= 5000 on day 10, else Bad.
-- The "debt collector" is NOT a quest -- it's this computed condition.
CREATE TYPE ending_type AS ENUM ('None', 'Good', 'Bad');

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
    ending                  ending_type NOT NULL DEFAULT 'None',

    created_at              TIMESTAMP NOT NULL DEFAULT NOW()
);