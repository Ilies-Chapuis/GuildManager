# Legacy of Tartaros — Guild Manager

Projet B2 La Plateforme — jeu de gestion et de simulation de guilde d'aventuriers, en **C# / WPF** avec une **API ASP.NET Core**.

> Après des années à arpenter les donjons les plus sombres de Tartaros, l'heure est venue de ranger l'épée. Fort de votre expérience, de votre renommée et de votre bourse bien remplie, fondez votre propre Guilde d'Aventuriers : recrutez la relève, gérez les ressources, négociez avec les marchands locaux, et envoyez la nouvelle génération accomplir les quêtes du royaume.

---

## Sommaire

- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Architecture du projet](#architecture-du-projet)
- [Prérequis](#prérequis)
- [Lancer le projet](#lancer-le-projet)
- [Équipe et répartition](#équipe-et-répartition)
- [Choix techniques](#choix-techniques)
- [Assets](#assets)
- [Base de données et API (mode en ligne)](#base-de-données-et-api-mode-en-ligne)
- [Documentation complémentaire](#documentation-complémentaire)

---

## Aperçu

Le joueur dirige une guilde pendant **10 jours**. Chaque jour se déroule en deux phases :

- **Phase de planification** : consulter le roster et les quêtes disponibles, assigner des aventuriers, recruter, gérer les ressources, sauvegarder.
- **Phase de résolution** : les quêtes envoyées sont résolues, les récompenses (or, XP, butin) sont distribuées, de nouvelles quêtes et de nouveaux candidats au recrutement sont générés pour le jour suivant.

Trois PNJ improbables (un Âne, un Chat, un Pigeon) apparaissent aux jours 1, 2 et 3 avec une proposition louche ; l'accepter bascule **définitivement** la narration du jeu vers une voix "WTF" plus loufoque (sinon elle reste "Sérieuse"). Aux jours 5 et 10, un combat de boss se débloque. Au jour 10, la guilde doit avoir réuni assez d'or pour éviter la mauvaise fin.

## Fonctionnalités

Correspondance avec le sujet du projet :

| Exigence du sujet | Statut | Détail |
|---|---|---|
| Aventuriers avec statistiques | ✅ | `Adventurer` et ses sous-classes (`Warrior`, `Healer`, `Mage`) |
| Aventuriers spéciaux (stats fixes, histoire, événements uniques) | ✅ | `Sameth`, `Meloap`, `Elowen` — chacun avec dialogue dédié |
| Aventuriers spéciaux non dupliables | ✅ | Instanciés une seule fois à la création de la partie |
| Quêtes avec restrictions/contraintes | ✅ | Taille d'équipe, classe requise, minimum de personnages spéciaux (boss) |
| Quêtes impactant la suite de la partie | ✅ | Système de réputation : les échecs réduisent l'or des quêtes futures |
| Conséquences d'un échec de quête | ✅ | Blessure (PV à 30%) ou mort (aventuriers génériques uniquement) |
| Dialogues interactifs déclenchés par condition | ✅ | PNJ improbables (jours 1-3), énigme de l'Âne (jour 4), rencontres de boss (jours 5/10) |
| Choix de dialogue influençant la partie | ✅ | Énigme du jour 4 : gain ou perte réelle d'or/nourriture selon la réponse |
| Ressources épuisables | ✅ | Or, nourriture, potions de vie |
| Condition de défaite | ✅ | Moins de 5000 or au jour 10 |
| Sens de progression | ✅ | Difficulté et récompenses des quêtes croissantes, aventuriers spéciaux à 80-85% pour compenser des recrues génériques plus faibles |
| Phase de planification complète | ✅ | Consultation aventuriers/quêtes, assignation, sauvegarde/quitter |
| Phase de résolution complète | ✅ | Résolution, récompenses, génération de quêtes/aventuriers |
| Mode solo (hors-ligne) | ✅ | Entièrement fonctionnel, sauvegarde locale en JSON (3 emplacements) |
| Système de comptes (inscription / connexion) | ✅ | Écran « En ligne » → API ASP.NET Core → base PostgreSQL (mot de passe haché avec BCrypt) |
| Mode en ligne (jeu complet via l'API) | ⚠️ | Le serveur et la base gèrent guildes partagées, recrutement, quêtes, jours et fin de partie (démontrable via `GuildManager.Api.http`), mais les écrans de jeu WPF ne l'utilisent pas encore — voir [Base de données et API](#base-de-données-et-api-mode-en-ligne) |
| Tests unitaires | ✅ | Projet xUnit `Tests/` : 77 tests (Guild, Quest, QuestResolver, Adventurer, GuildResources, NarrativeManager, UpkeepCalculator, clients API) — `dotnet test Tests/GuildManager.Tests.csproj` |

## Architecture du projet

```
Guild_Manager/
├── Logic/                          Bibliothèque de logique métier (aucune dépendance à WPF)
│   ├── Gameplay/                   Aventuriers, quêtes, ressources, narration, sauvegarde
│   ├── MainMenu/                   GameSessionManager (pont entre la logique et l'UI)
│   ├── API/GuildManager.Api/       Serveur ASP.NET Core + EF Core (mode en ligne)
│   ├── API/*.cs                    Clients HTTP côté jeu (AuthApiClient, GuildApiClient, QuestApiClient)
│   ├── API/Dtos/                   DTO de requêtes/réponses partagés entre le serveur et les clients du jeu
│   ├── Database/                   Scripts SQL PostgreSQL (schéma) + setup-database.ps1
│   └── Data/                       Données JSON (dialogues, quêtes, noms, recrues génériques)
│
├── GuildManager.csproj             Application WPF principale (mode solo)
│   ├── View/ + Controls/           Fichiers XAML des écrans
│   ├── UI/                         Code-behind des écrans
│   ├── Dialogues/                  Système d'affichage des dialogues (portraits, boîtes)
│   └── Assets/                     Images, sprites, planches de portraits
│
├── Demo/                           Harnais de test en console (démonstration de la logique seule)
├── Graphic/                        Prototype WPF indépendant (non utilisé dans le jeu final)
├── Tests/                          Tests unitaires (xUnit)
└── docker-compose.yml              PostgreSQL local avec le schéma créé automatiquement
```

La logique de jeu (`Logic/Gameplay/`) est volontairement indépendante de toute interface graphique, afin de pouvoir être testée et réutilisée sans dépendre de WPF (c'est ce qui permet au harnais `Demo` en console de fonctionner avec exactement le même code que le vrai jeu).

## Prérequis

- [.NET SDK](https://dotnet.microsoft.com/download) (net10.0 / net10.0-windows)
- Windows (le projet WPF principal nécessite `net10.0-windows`)
- *Mode en ligne uniquement* : PostgreSQL 16 (installé localement) **ou** Docker — voir [Base de données et API](#base-de-données-et-api-mode-en-ligne). Le mode solo n'en a pas besoin.

## Lancer le projet

Depuis la racine du projet :

```powershell
dotnet build
dotnet run --project GuildManager.csproj
```

Pour lancer le harnais de test en console (logique seule, sans interface graphique) :

```powershell
dotnet run --project Demo
```

## Équipe et répartition

| Membre | Responsabilité |
|---|---|
| Melvin | Logique de jeu (`Logic/Gameplay/`) : aventuriers, quêtes, ressources, narration, sauvegarde |
| Guillaume | Interface WPF (écrans, navigation, intégration graphique) |
| Illies | API ASP.NET Core et base de données (mode en ligne) |

## Choix techniques

- **Séparation Logic / UI** : `GuildManager.Logic` est une bibliothèque de classes sans référence à WPF, référencée par le projet WPF (`GuildManager.csproj`), le harnais console (`Demo`) et potentiellement l'API. Ce choix permet de tester et faire évoluer la logique de jeu indépendamment de l'interface.
- **DTO partagés** : les classes de requêtes/réponses de l'API existent une seule fois, dans `Logic/API/Dtos`, et sont compilées à la fois dans le jeu (`GuildManager.Logic`) et dans le serveur (`GuildManager.Api.csproj` inclut `..\Dtos\*.cs`). Une modification d'un format JSON ne peut donc pas diverger entre client et serveur.
- **`GameSessionManager`** : point d'entrée unique côté UI pour accéder à la partie en cours, plutôt que de faire circuler une instance de `Guild` entre tous les écrans.
- **Sauvegarde en JSON** : simple à inspecter/déboguer, et suffisante pour un mode solo local. Les chemins de fichiers sont résolus par rapport au dossier de l'exécutable (`AppContext.BaseDirectory`) plutôt qu'au répertoire de travail courant, pour fonctionner de manière fiable quel que soit le mode de lancement (IDE, `dotnet run`, double-clic).
- **Système de dialogue séparé** : `NarrativeManager` (quel texte afficher, quelle voix) est indépendant du système d'affichage visuel (`Dialogues/`, portraits et boîtes de dialogue façon Fire Emblem), pour que la logique de narration reste testable sans interface.

## Assets

*(à compléter par l'équipe graphique : outils de génération IA utilisés pour les illustrations, planches de portraits et arrière-plans, et justification du choix — exigence du sujet, section "Assets")*

## Base de données et API (mode en ligne)

Le mode en ligne repose sur un serveur **ASP.NET Core** (`Logic/API/GuildManager.Api`, minimal API + Entity Framework Core) branché sur une base **PostgreSQL** dont le schéma est décrit par les scripts `Logic/Database/*.sql` (6 tables : `players`, `guilds`, `guild_memberships`, `adventurers`, `quests`, `quest_assignments`).

**Pourquoi PostgreSQL ?** Base relationnelle standard, open source, avec les contraintes dont on a besoin directement dans le schéma (unicité d'un personnage spécial par guilde, jour entre 1 et 10, or ≥ 0, types de quêtes valides…). Les règles d'intégrité sont donc garanties par la base, pas seulement par le code.

### 1. Lancer la base de données

Au choix :

```powershell
# Option A — Docker (le schéma est créé automatiquement au premier démarrage)
docker compose up -d

# Option B — PostgreSQL installé localement (psql dans le PATH, utilisateur postgres / mot de passe postgres)
.\Logic\Database\setup-database.ps1
```

La chaîne de connexion est dans `Logic/API/GuildManager.Api/appsettings.json` (base `guildmanager`, utilisateur `postgres`). Elle peut être surchargée par la variable d'environnement `ConnectionStrings__GuildManagerDb`.

### 2. Lancer l'API

```powershell
dotnet run --project Logic/API/GuildManager.Api
```

Le serveur écoute sur `http://localhost:5211` (adresse modifiable côté jeu avec la variable d'environnement `GUILDMANAGER_API_URL`).

### 3. Utiliser le mode en ligne

- **Depuis le jeu** : menu principal → *En ligne* → *Inscription* puis *Connexion*. Le compte est réellement créé dans la table `players` (l'API exige un email : il est généré à partir du pseudo, sous la forme `pseudo@guildmanager.local`).
- **Parcours complet via l'API** : ouvrir `Logic/API/GuildManager.Api/GuildManager.Api.http` (extension *REST Client* de VS Code) et exécuter les requêtes dans l'ordre : inscription, création de guilde, recrutement, génération des quêtes par l'API, envoi d'une équipe, jour suivant, évaluation de la fin.

### État actuel et limites

- ✅ Comptes (inscription/connexion) branchés du jeu jusqu'à la base.
- ✅ Génération des quêtes, résolution, recrutement, ressources partagées et progression des jours faits **côté serveur**, conformément au mode « coop » du sujet ; les clients HTTP du jeu (`AuthApiClient`, `GuildApiClient`, `QuestApiClient`) sont écrits et testés.
- ⚠️ Les écrans de jeu WPF (quêtes, recrutement, etc.) fonctionnent avec la logique locale (mode solo) et n'appellent pas encore l'API : le jeu complet en ligne n'est donc pas jouable depuis l'interface.
- ⚠️ Les règles côté serveur sont une version simplifiée de celles du solo (récompenses d'or et d'XP plus basses, pas de coût en nourriture par quête, combat de boss plus simple). À aligner si le mode en ligne est poursuivi.

## Documentation complémentaire

Voir [`guide_soutenance.md`](./guide_soutenance.md) pour une explication détaillée, classe par classe et méthode par méthode, de toute la logique de jeu — utile pour préparer ou suivre la soutenance.
