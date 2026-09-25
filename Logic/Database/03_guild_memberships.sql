-- ============================================================
-- 3. GUILD_MEMBERSHIPS (several players -> one guild)
-- ============================================================

CREATE TABLE guild_memberships (
    player_id   INTEGER NOT NULL REFERENCES players(id) ON DELETE CASCADE,
    guild_id    INTEGER NOT NULL REFERENCES guilds(id) ON DELETE CASCADE,
    joined_at   TIMESTAMP NOT NULL DEFAULT NOW(),

    PRIMARY KEY (player_id, guild_id)
);