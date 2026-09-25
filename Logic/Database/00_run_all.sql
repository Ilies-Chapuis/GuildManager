-- ============================================================
-- Runs every file in order. Usage:
--   psql -U your_user -d your_database -f 00_run_all.sql
-- (run from inside the sql/ folder, or adjust the paths below)
-- ============================================================

\i 01_players.sql
\i 02_guilds.sql
\i 03_guild_memberships.sql
\i 04_adventurers.sql
\i 05_quests.sql
\i 06_quest_assignments.sql