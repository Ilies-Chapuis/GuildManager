-- ============================================================
-- Runs every schema file in order. Usage (from any directory):
--   psql -U postgres -d guildmanager -f 00_run_all.sql
-- \ir resolves each path relative to this file, not to the current
-- directory, so the command above works wherever it is launched from.
-- The scripts use CREATE TABLE IF NOT EXISTS, so re-running is harmless.
-- ============================================================

\ir 01_players.sql
\ir 02_guilds.sql
\ir 03_guild_memberships.sql
\ir 04_adventurers.sql
\ir 05_quests.sql
\ir 06_quest_assignments.sql
