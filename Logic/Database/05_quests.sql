-- ============================================================
-- 5. QUESTS
-- Matches GuildManager.Logic.Gameplay.Quests.Quest (C#).
-- ============================================================

CREATE TYPE quest_type AS ENUM (
    'Escort',
    'Exorcism',
    'DungeonExploration',
    'SpecialAdventurer',
    'ImprobableNpc',
    'BossFight'
);

CREATE TABLE quests (
    id                                 SERIAL PRIMARY KEY,
    guild_id                           INTEGER NOT NULL REFERENCES guilds(id) ON DELETE CASCADE,

    -- NULL for guild-wide/shared quests (e.g. boss fights),
    -- filled in for a quest personal to one player
    owner_player_id                    INTEGER REFERENCES players(id) ON DELETE CASCADE,

    name                                VARCHAR(150) NOT NULL,
    day_available                       INTEGER NOT NULL CHECK (day_available BETWEEN 1 AND 10),
    type                                quest_type NOT NULL,
    difficulty                          INTEGER NOT NULL,
    duration_hours                      INTEGER NOT NULL,
    gold_reward                         INTEGER NOT NULL DEFAULT 0,

    minimum_level_required              INTEGER NOT NULL DEFAULT 1,
    minimum_team_size                   INTEGER NOT NULL DEFAULT 1,
    maximum_team_size                   INTEGER NOT NULL DEFAULT 5,

    -- Boss fights: team must contain at least one Warrior, one Healer, one Mage
    requires_all_recruit_categories     BOOLEAN NOT NULL DEFAULT FALSE,

    -- 'Warrior' / 'Healer' / 'Mage', NULL = no specific class required
    required_class_name                 VARCHAR(20)
        CHECK (required_class_name IS NULL OR required_class_name IN ('Warrior', 'Healer', 'Mage')),

    -- Mutable state, matches Quest.MarkResolved() / RestoreResolution()
    is_resolved                         BOOLEAN NOT NULL DEFAULT FALSE,
    was_successful                      BOOLEAN, -- NULL until resolved

    created_at                          TIMESTAMP NOT NULL DEFAULT NOW()
);