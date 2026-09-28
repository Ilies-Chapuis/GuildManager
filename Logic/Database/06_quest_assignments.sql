-- ============================================================
-- 6. QUEST_ASSIGNMENTS (several adventurers -> one quest)
-- ============================================================

CREATE TABLE IF NOT EXISTS quest_assignments (
    quest_id        INTEGER NOT NULL REFERENCES quests(id) ON DELETE CASCADE,
    adventurer_id   INTEGER NOT NULL REFERENCES adventurers(id) ON DELETE CASCADE,
    assigned_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    PRIMARY KEY (quest_id, adventurer_id)
);