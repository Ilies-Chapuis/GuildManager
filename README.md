# Legacy of Tartaros — Guild Manager

B2 project at La Plateforme — an adventurers' guild management and simulation game, built with **C# / WPF** and an **ASP.NET Core API**.

> After years spent crawling the darkest dungeons of Tartaros, the time has come to put down your sword. With your experience, your renown and a well-filled purse, you decide to found your own Adventurers' Guild: recruit the next generation, manage the guild's resources, deal with local merchants, and send your recruits off to complete the kingdom's quests.

*The game itself (interface, dialogues, quest texts) is in French; the code and its comments are in English.*

---

## Table of contents

- [Overview](#overview)
- [Features](#features)
- [Project architecture](#project-architecture)
- [Requirements](#requirements)
- [Running the project](#running-the-project)
- [Team and responsibilities](#team-and-responsibilities)
- [Technical choices](#technical-choices)
- [Assets](#assets)
- [Database and API (online mode)](#database-and-api-online-mode)
- [Additional documentation](#additional-documentation)

---

## Overview

The player runs a guild for **10 days**. Each day has two phases:

- **Planning phase**: browse the roster and the available quests, assign adventurers, recruit, manage resources, save.
- **Resolution phase**: the quests that were sent are resolved, rewards (gold, XP, loot) are handed out, and new quests and new recruitment candidates are generated for the next day.

Three improbable NPCs (a Donkey, a Cat and a Pigeon) show up on days 1, 2 and 3 with a shady offer; accepting one **permanently** switches the game's narration to a goofier "WTF" voice (otherwise it stays "Serious"). Boss fights unlock on days 5 and 10. On day 10 the guild must have gathered enough gold to avoid the bad ending.

## Features

Mapping to the project brief:

| Requirement | Status | Details |
|---|---|---|
| Adventurers with stats | ✅ | `Adventurer` and its subclasses (`Warrior`, `Healer`, `Mage`) |
| Special adventurers (fixed stats, backstory, unique events) | ✅ | `Sameth`, `Meloap`, `Elowen` — each with dedicated dialogue |
| Special adventurers can't be duplicated | ✅ | Instantiated only once, when the game is created |
| Quests with restrictions/constraints | ✅ | Team size, required class, minimum number of special adventurers (boss fights) |
| Quests that affect the rest of the game | ✅ | Reputation system: failures reduce the gold of future quests |
| Consequences of a failed quest | ✅ | Injury (HP dropped to 30%) or death (generic adventurers only) |
| Interactive dialogues triggered by conditions | ✅ | Improbable NPCs (days 1-3), the Donkey's riddle (day 4), boss encounters (days 5/10) |
| Dialogue choices that influence the game | ✅ | Day-4 riddle: real gold/food gained or lost depending on the answer |
| Depletable resources | ✅ | Gold, food, health potions |
| Defeat condition | ✅ | Less than 5000 gold on day 10 |
| Sense of progression | ✅ | Quest difficulty and rewards grow over time; special adventurers at 80-85% to offset weaker generic recruits |
| Complete planning phase | ✅ | Browse adventurers/quests, assign, save/quit |
| Complete resolution phase | ✅ | Resolution, rewards, quest/adventurer generation |
| Solo mode (offline) | ✅ | Fully working, local JSON save (3 slots) |
| Account system (register / login) | ✅ | "En ligne" (Online) screen → ASP.NET Core API → PostgreSQL database (password hashed with BCrypt) |
| Online mode (full game through the API) | ⚠️ | The server and database handle shared guilds, recruiting, quests, days and the ending (demonstrable with `GuildManager.Api.http`), but the WPF game screens don't use them yet — see [Database and API](#database-and-api-online-mode) |
| Unit tests | ✅ | xUnit project `Tests/`: 77 tests (Guild, Quest, QuestResolver, Adventurer, GuildResources, NarrativeManager, UpkeepCalculator, API clients) — `dotnet test Tests/GuildManager.Tests.csproj` |

## Project architecture

```
Guild_Manager/
├── Logic/                          Business-logic library (no WPF dependency)
│   ├── Gameplay/                   Adventurers, quests, resources, narration, saving
│   ├── MainMenu/                   GameSessionManager (bridge between the logic and the UI)
│   ├── API/GuildManager.Api/       ASP.NET Core + EF Core server (online mode)
│   ├── API/*.cs                    Game-side HTTP clients (AuthApiClient, GuildApiClient, QuestApiClient)
<<<<<<< HEAD
│   ├── API/Dtos/                   Request/response DTOs shared by the server and the game clients
=======
>>>>>>> 36d7a65a2a5d426c90d142dd5831c85c11ca72c1
│   ├── Database/                   PostgreSQL SQL scripts (schema) + setup-database.ps1
│   └── Data/                       JSON data (dialogues, quests, names, generic recruits)
│
├── GuildManager.csproj             Main WPF application (solo mode)
│   ├── View/ + Controls/           XAML files for the screens
│   ├── UI/                         Code-behind for the screens
│   ├── Dialogues/                  Dialogue display system (portraits, boxes)
│   └── Assets/                     Images, sprites, portrait sheets
│
├── Demo/                           Console test harness (runs the logic alone)
├── Graphic/                        Standalone WPF prototype (not used in the final game)
├── Tests/                          Unit tests (xUnit)
└── docker-compose.yml              Local PostgreSQL with the schema created automatically
```

The game logic (`Logic/Gameplay/`) is deliberately independent from any graphical interface, so it can be tested and reused without WPF (this is what lets the `Demo` console harness run with exactly the same code as the real game).

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) (net10.0 / net10.0-windows)
- Windows (the main WPF project targets `net10.0-windows`)
- *Online mode only*: PostgreSQL 16 (installed locally) **or** Docker — see [Database and API](#database-and-api-online-mode). Solo mode doesn't need either.

## Running the project

From the project root:

```powershell
dotnet build
dotnet run --project GuildManager.csproj
```

To run the console test harness (logic only, no graphical interface):

```powershell
dotnet run --project Demo
```

## Team and responsibilities

| Member | Responsibility |
|---|---|
| Melvin | Game logic (`Logic/Gameplay/`): adventurers, quests, resources, narration, saving |
| Guillaume | WPF interface (screens, navigation, graphics integration) |
| Illies | ASP.NET Core API and database (online mode) |

## Technical choices

- **Logic / UI separation**: `GuildManager.Logic` is a class library with no reference to WPF, referenced by the WPF project (`GuildManager.csproj`), the console harness (`Demo`) and the tests. This lets the game logic be tested and evolved independently of the interface.
<<<<<<< HEAD
- **Shared DTOs**: the request/response classes of the API live once, in `Logic/API/Dtos`, and are compiled into both the game (`GuildManager.Logic`) and the server (`GuildManager.Api.csproj` includes `..\Dtos\*.cs`). A change to a JSON shape therefore can't drift between client and server.
=======
>>>>>>> 36d7a65a2a5d426c90d142dd5831c85c11ca72c1
- **`GameSessionManager`**: a single entry point on the UI side to reach the current game, instead of passing a `Guild` instance around between every screen.
- **JSON saves**: easy to inspect/debug, and sufficient for a local solo mode. File paths are resolved relative to the executable's folder (`AppContext.BaseDirectory`) rather than the current working directory, so they behave the same however the game is launched (IDE, `dotnet run`, double-click).
- **Separate dialogue system**: `NarrativeManager` (which text, which voice) is independent from the visual display system (`Dialogues/`, Fire Emblem-style portraits and dialogue boxes), so the narration logic stays testable without any UI.

## Assets

*(to be completed by the graphics team: AI generation tools used for the illustrations, portrait sheets and backgrounds, and the reasons for choosing them — requirement of the project brief, "Assets" section)*

## Database and API (online mode)

The online mode relies on an **ASP.NET Core** server (`Logic/API/GuildManager.Api`, minimal API + Entity Framework Core) connected to a **PostgreSQL** database whose schema is defined by the scripts in `Logic/Database/*.sql` (6 tables: `players`, `guilds`, `guild_memberships`, `adventurers`, `quests`, `quest_assignments`).

**Why PostgreSQL?** A standard, open-source relational database that lets us put the integrity rules directly in the schema (one special adventurer of each kind per guild, day between 1 and 10, gold ≥ 0, valid quest types…). Those rules are therefore enforced by the database itself, not only by the code.

### 1. Start the database

Pick one:

```powershell
# Option A — Docker (the schema is created automatically on first start)
docker compose up -d

# Option B — PostgreSQL installed locally (psql on the PATH, user postgres / password postgres)
.\Logic\Database\setup-database.ps1
```

The connection string lives in `Logic/API/GuildManager.Api/appsettings.json` (database `guildmanager`, user `postgres`). It can be overridden with the `ConnectionStrings__GuildManagerDb` environment variable.

### 2. Start the API

```powershell
dotnet run --project Logic/API/GuildManager.Api
```

The server listens on `http://localhost:5211` (the address the game uses can be changed with the `GUILDMANAGER_API_URL` environment variable).

### 3. Use the online mode

- **From the game**: main menu → *En ligne* (Online) → *Inscription* (Register), then *Connexion* (Login). The account is really created in the `players` table (the API requires an email, so one is generated from the username, in the form `username@guildmanager.local`).
- **Full flow through the API**: open `Logic/API/GuildManager.Api/GuildManager.Api.http` (VS Code *REST Client* extension) and run the requests in order: register, create a guild, recruit, let the API generate the quests, send a team, next day, evaluate the ending.

### Current status and limitations

- ✅ Accounts (register/login) are wired from the game all the way to the database.
- ✅ Quest generation, resolution, recruiting, shared resources and day progression are done **server-side**, as the brief requires for the "coop" mode; the game's HTTP clients (`AuthApiClient`, `GuildApiClient`, `QuestApiClient`) are written and tested.
- ⚠️ The WPF game screens (quests, recruiting, etc.) run on the local logic (solo mode) and don't call the API yet, so the full online game isn't playable from the interface.
- ⚠️ The server-side rules are a simplified version of the solo ones (lower gold and XP rewards, no food cost per quest, simpler boss fight). They should be aligned if the online mode is pursued.

<<<<<<< HEAD
## Additional documentation

See [`guide_soutenance.md`](./guide_soutenance.md) (in French) for a detailed, class-by-class and method-by-method explanation of all the game logic — useful to prepare for or follow the defense. The previous French version of this README is kept as [`README.fr.md`](./README.fr.md).
=======

>>>>>>> 36d7a65a2a5d426c90d142dd5831c85c11ca72c1
