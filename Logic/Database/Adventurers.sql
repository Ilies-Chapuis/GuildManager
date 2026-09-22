-- Enum pour la catégorie d'aventurier (Warrior/Healer/Mage/Special)
CREATE TYPE adventurer_category AS ENUM ('Warrior', 'Healer', 'Mage', 'Special');

-- Enum pour le statut de l'aventurier
CREATE TYPE adventurer_status AS ENUM ('Available', 'OnQuest', 'Dead', 'Injured');

CREATE TABLE adventurers (
    id                      SERIAL PRIMARY KEY,
    guild_id                INTEGER NOT NULL REFERENCES guilds(id) ON DELETE CASCADE,
    owner_player_id         INTEGER NOT NULL REFERENCES players(id) ON DELETE CASCADE,

    name                    VARCHAR(100) NOT NULL,
    category                adventurer_category NOT NULL,
    special_key             VARCHAR(50), -- 'Elowen' / 'Meloap' / 'Sameth', NULL si pas spécial

    current_health_points   INTEGER NOT NULL,
    level                   INTEGER NOT NULL DEFAULT 1,
    experience              INTEGER NOT NULL DEFAULT 0,
    unique_event_triggered  BOOLEAN, -- NULL si pas un Special, sinon true/false

    status                  adventurer_status NOT NULL DEFAULT 'Available',

    created_at              TIMESTAMP NOT NULL DEFAULT NOW(),

    -- Empêche un doublon d'aventurier spécial dans la même guilde
    CONSTRAINT unique_special_per_guild UNIQUE (guild_id, special_key)
);