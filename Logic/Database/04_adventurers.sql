-- ============================================================
-- 4. ADVENTURERS
-- ============================================================

-- Category: playable class (Warrior/Healer/Mage) or special adventurer
CREATE TYPE adventurer_category AS ENUM ('Warrior', 'Healer', 'Mage', 'Special');

-- 'Dead' is kept as a status (soft delete) rather than removing the row,
-- even though Guild.AttemptQuest() removes dead recruits from the
-- in-memory Roster list -- confirm this approach with the team.
CREATE TYPE adventurer_status AS ENUM ('Available', 'OnQuest', 'Dead', 'Injured');

CREATE TABLE adventurers (
    id                      SERIAL PRIMARY KEY,
    guild_id                INTEGER NOT NULL REFERENCES guilds(id) ON DELETE CASCADE,
    owner_player_id         INTEGER NOT NULL REFERENCES players(id) ON DELETE CASCADE,

    name                    VARCHAR(100) NOT NULL,
    category                adventurer_category NOT NULL,

    -- 'Elowen' / 'Meloap' / 'Sameth' if category = 'Special', NULL otherwise
    special_key             VARCHAR(50),

    -- Mutable state, matches RestoreProgress() on the C# side
    current_health_points   INTEGER NOT NULL,
    level                   INTEGER NOT NULL DEFAULT 1,
    experience              INTEGER NOT NULL DEFAULT 0,

    -- Mutable state, matches RestoreSpecialProgress() (Special only)
    unique_event_triggered  BOOLEAN,

    -- Matches Adventurer.UsedToday / MarkUsedToday() / ResetDailyUsage():
    -- a recruit can only go on one quest per day. Reset to FALSE at the
    -- start of each new day (see Guild.ResetDailyUsage()).
    used_today              BOOLEAN NOT NULL DEFAULT FALSE,

    status                  adventurer_status NOT NULL DEFAULT 'Available',

    created_at              TIMESTAMP NOT NULL DEFAULT NOW(),

    -- Prevents a duplicate special adventurer within the same guild
    -- (NULL != NULL in SQL, so this does not block non-special adventurers)
    CONSTRAINT unique_special_per_guild UNIQUE (guild_id, special_key)
);