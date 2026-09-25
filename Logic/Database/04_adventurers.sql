-- ============================================================
-- 4. ADVENTURERS
-- ============================================================

CREATE TABLE adventurers (
    id                      SERIAL PRIMARY KEY,
    guild_id                INTEGER NOT NULL REFERENCES guilds(id) ON DELETE CASCADE,
    owner_player_id         INTEGER NOT NULL REFERENCES players(id) ON DELETE CASCADE,

    name                    VARCHAR(100) NOT NULL,

    -- Playable class (Warrior/Healer/Mage) or special adventurer
    category                VARCHAR(10) NOT NULL
        CHECK (category IN ('Warrior', 'Healer', 'Mage', 'Special')),

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

    -- 'Dead' is kept as a status (soft delete) rather than removing the
    -- row, even though Guild.AttemptQuest() removes dead recruits from
    -- the in-memory Roster list.
    status                  VARCHAR(10) NOT NULL DEFAULT 'Available'
        CHECK (status IN ('Available', 'OnQuest', 'Dead', 'Injured')),

    created_at              TIMESTAMP NOT NULL DEFAULT NOW(),

    -- Prevents a duplicate special adventurer within the same guild
    -- (NULL != NULL in SQL, so this does not block non-special adventurers)
    CONSTRAINT unique_special_per_guild UNIQUE (guild_id, special_key)
);