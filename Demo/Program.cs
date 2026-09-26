using System;
using GuildManager.Demo;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;
using GuildManager.Logic.Gameplay.Quests;
using GuildManager.Logic.Gameplay.SaveSystem;

// TMP: entry-point menu 
Console.WriteLine("Choose a mode:");
Console.WriteLine("1: Feature demos (existing step-by-step walkthrough)");
Console.WriteLine("2: Full game loop (TMP terminal harness, day 1 to 10)");
Console.Write("> ");

if (Console.ReadLine()?.Trim() == "2")
{
    TerminalGameLoop.Run();
    return;
}

// FR : mode par défaut (ou tout choix différent de "2") -> les démos par fonctionnalité ci-dessous
var narrativeManager = new NarrativeManager();

// The switch is a one-shot event: we listen for it to react at the exact
// moment the game permanently switches voice (GDD 4.2).
// FR : La bascule est un événement unique : on l'écoute pour réagir au moment précis où la voix change définitivement.
narrativeManager.OnSwitchToWtf += npc =>
    Console.WriteLine($"\n>>> You accepted {npc.Name}'s quest. " +
                       "Permanent switch to the Wtf voice for the rest of the game. <<<\n");

var sameth = new Sameth();
var meloap = new Meloap();
var elowen = new Elowen();

Console.WriteLine("=== Day 1 ===");
Console.WriteLine(narrativeManager.GetText(DialogueRepository.Find("guild_founding")!));

Console.WriteLine($"\nAn improbable NPC approaches: {ImprobableNpc.BlackMarketDonkey.Name}");
Console.WriteLine(ImprobableNpc.BlackMarketDonkey.Appearance);
Console.WriteLine($"Quest offered: {ImprobableNpc.BlackMarketDonkey.QuestHook}");

Console.Write("\nAccept the quest? (y/n): ");
string? answer = Console.ReadLine();

if (answer?.Trim().ToLower() == "y")
    narrativeManager.AcceptImprobableNpcQuest(ImprobableNpc.BlackMarketDonkey);
else
    narrativeManager.DeclineImprobableNpcQuest();

Console.WriteLine($"\nActive voice for the rest of the game: {narrativeManager.CurrentVoice}");

Console.WriteLine("\n=== Day 4 ===");
Console.WriteLine($"{sameth.Name}: {narrativeManager.GetAdventurerText(sameth, day: 4)}");

Console.WriteLine("\n=== Day 5 ===");
Console.WriteLine($"{meloap.Name}: {narrativeManager.GetAdventurerText(meloap, day: 5)}");

Console.WriteLine("\n=== Day 10 ===");
Console.WriteLine(narrativeManager.GetText(DialogueRepository.Find("day10_ledger")!));

// Example use of Elowen and the quest resolution system (GDD 6.4)
// FR : Exemple d'utilisation d'Elowen et du système de résolution de quêtes.
int exorcismDifficulty = 40;
Console.WriteLine($"\n{elowen.Name} attempts an exorcism quest (difficulty {exorcismDifficulty}): " +
                   $"{elowen.CalculateSuccessRate(exorcismDifficulty)}% estimated success rate.");

// --- Guild: roster, resources, day cycle ---
// FR : --- Guilde : roster, ressources, cycle de jour ---
Console.WriteLine("\n=== Day cycle and quests ===");
var guild = new Guild();
guild.RecruitAdventurer(sameth);
guild.RecruitAdventurer(meloap);
guild.RecruitAdventurer(elowen);

bool wellFed = guild.ApplyDailyUpkeep();
Console.WriteLine($"Daily upkeep paid: {wellFed} (remaining food: {guild.Resources.Food})");

var escortQuest = new Quest("Caravan escort", QuestType.Escort,
    difficulty: 30, durationHours: 4, goldReward: 100);

// A single recruit
// FR : Une seule recrue
bool soloResult = guild.AttemptQuest(escortQuest, meloap);
Console.WriteLine($"{escortQuest.Name} attempted by {meloap.Name} alone: " +
                   $"{(soloResult ? "success" : "failure")} " +
                   $"- remaining hours today: {guild.Cycle.RemainingHours}h" +
                   $" - guild gold: {guild.Resources.Gold}");

// A separate quest, attempted by two: the team bonus applies
// FR : Une autre quête, tentée à deux : le bonus d'équipe s'applique
var dungeonQuest = new Quest("Dungeon exploration", QuestType.DungeonExploration,
    difficulty: 50, durationHours: 8, goldReward: 250);

bool groupResult = guild.AttemptQuest(dungeonQuest, new Adventurer[] { meloap, elowen });
Console.WriteLine($"{dungeonQuest.Name} attempted by {meloap.Name} + {elowen.Name}: " +
                   $"{(groupResult ? "success" : "failure")} " +
                   $"- remaining hours today: {guild.Cycle.RemainingHours}h" +
                   $" - guild gold: {guild.Resources.Gold}");

// A lone Healer: guaranteed 0% success rate, it does not fight the quest itself
// FR : Un Healeur seul : 0% de réussite garanti, il ne combat pas la quête lui-même
var healer = new Healer("Elara");
var easyQuest = new Quest("Local delivery", QuestType.Escort,
    difficulty: 10, durationHours: 2, goldReward: 30);

bool healerAloneResult = guild.AttemptQuest(easyQuest, healer);
Console.WriteLine($"\n{easyQuest.Name} attempted by {healer.Name} alone: " +
                   $"{(healerAloneResult ? "success" : "failure")} (always a failure, as expected)");

// The same Healer, but accompanied: its x1.2 multiplier applies this time
// FR : Le même Healeur, mais accompagné : son multiplicateur x1.2 s'applique cette fois
var easyQuest2 = new Quest("Local delivery (bis)", QuestType.Escort,
    difficulty: 10, durationHours: 2, goldReward: 30);

bool healerWithGroupResult = guild.AttemptQuest(easyQuest2, new Adventurer[] { healer, meloap });
Console.WriteLine($"{easyQuest2.Name} attempted by {healer.Name} + {meloap.Name}: " +
                   $"{(healerWithGroupResult ? "success" : "failure")}");

// --- Save / load (solo mode) ---
// FR : --- Sauvegarde / chargement (mode solo) ---
Console.WriteLine("\n=== Save ===");
SaveManager.Save(guild, "Saves/save.json");
Console.WriteLine("Game saved to Saves/save.json");

var loadedGuild = SaveManager.Load("Saves/save.json");
Console.WriteLine($"Game reloaded: day {loadedGuild.Cycle.CurrentDay}, " +
                   $"{loadedGuild.Resources.Gold} gold, " +
                   $"{loadedGuild.Resources.Food} food, " +
                   $"{loadedGuild.Resources.HealthPotions} health potions, " +
                   $"{loadedGuild.Roster.Count} recruits in the roster.");

// --- Health potions: healing a recruit ---
// FR : --- Potions de vie : soin d'une recrue ---
Console.WriteLine("\n=== Health potions ===");
meloap.Damage(20); // simulates damage taken during a quest
Console.WriteLine($"{meloap.Name} before potion: {meloap.HealthPoints}/{meloap.MaxHealthPoints} HP, " +
                   $"potion stock: {guild.Resources.HealthPotions}");

bool potionUsed = guild.UseHealthPotion(meloap);
Console.WriteLine($"Potion used on {meloap.Name}: {potionUsed} " +
                   $"- {meloap.Name} now has {meloap.HealthPoints}/{meloap.MaxHealthPoints} HP, " +
                   $"remaining potion stock: {guild.Resources.HealthPotions}");

// --- Boss fights: both require a team of at least 4, one of each category ---
// FR : --- Combats de boss : équipe de 4 minimum, une de chaque catégorie, dans les deux cas ---
Console.WriteLine("\n=== Day 5 boss fight: Nyxaria ===");
Console.WriteLine(narrativeManager.GetText(DialogueRepository.Find("nyxaria_boss_encounter")!));

guild.Cycle.AdvanceToNextDay(); // fresh 16h budget for this demo section
// FR : nouveau budget de 16h pour cette section de démo

var nyxariaFight = new Quest("Nyxaria's Awakening", QuestType.BossFight,
    difficulty: 55, durationHours: 10, goldReward: 800,
    minimumTeamSize: 4, requiresAllRecruitCategories: true);

var warrior = new Warrior("Bram");
var mage = new Mage("Yssa");

// Attempt with only 3 recruits and a missing category: refused before any roll
bool incompleteTeamResult = guild.AttemptQuest(nyxariaFight, new Adventurer[] { warrior, healer, meloap });
Console.WriteLine($"Attempt with an incomplete team (3 recruits, no Mage): {incompleteTeamResult} " +
                   "(refused: team requirements not met)");

// Attempt with 4 recruits covering every category: requirements are met
bool fullTeamResult = guild.AttemptQuest(nyxariaFight, new Adventurer[] { warrior, healer, mage, meloap });
Console.WriteLine($"Attempt with a full team (Warrior + Healer + Mage + Meloap): " +
                   $"{(fullTeamResult ? "success" : "failure")}");

Console.WriteLine("\n=== Day 10 boss fight: Grendel ===");
Console.WriteLine(narrativeManager.GetText(DialogueRepository.Find("final_boss_showdown")!));

guild.Cycle.AdvanceToNextDay(); // fresh 16h budget for this demo section
// FR : nouveau budget de 16h pour cette section de démo

var grendelFight = new Quest("Grendel's Showdown", QuestType.BossFight,
    difficulty: 75, durationHours: 12, goldReward: 2000,
    minimumTeamSize: 4, requiresAllRecruitCategories: true);

bool grendelResult = guild.AttemptQuest(grendelFight, new Adventurer[] { warrior, healer, mage, sameth });
Console.WriteLine($"Attempt with a full team (Warrior + Healer + Mage + Sameth): " +
                   $"{(grendelResult ? "success" : "failure")}");
