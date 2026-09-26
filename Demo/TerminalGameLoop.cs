
// TMP - TEMPORARY terminal
using System;
using System.Collections.Generic;
using System.Linq;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;
using GuildManager.Logic.Gameplay.Quests;
using GuildManager.Logic.Gameplay.SaveSystem;

namespace GuildManager.Demo;

public static class TerminalGameLoop
{
    // TMP: flat hiring cost and exchange rate, until real balancing exists.
    // FR : TMP — coût de recrutement fixe et taux d'échange, en attendant un vrai équilibrage.
    private const int HireCost = 50;
    private const int GoldPerFoodUnit = 5;
    private const int GoldPerHealthPotion = 15;
    private const string SavePath = "Saves/save.json";

    // Quest generation and quest level now live in Logic/Gameplay/Quests/QuestGenerator.cs
    // FR : La génération de quêtes et son niveau vivent maintenant dans QuestGenerator.cs

    // TMP: one improbable NPC offered per day, for the first 3 days, until
    // one is accepted.
    // FR : TMP — un PNJ improbable par jour, sur les 3 premiers jours, jusqu'à acceptation.
    private static ImprobableNpc? NpcForDay(int day) => day switch
    {
        1 => ImprobableNpc.BlackMarketDonkey,
        2 => ImprobableNpc.MessengerPigeon,
        3 => ImprobableNpc.TalkingCat,
        _ => null
    };

    // TMP: readable class label for console output, since Adventurer.ToString()
    // does not include it.
    // FR : TMP — étiquette de classe lisible pour la console.
    private static string DescribeType(Adventurer recruit) => recruit switch
    {
        Warrior => "Warrior",
        Healer => "Healer",
        Mage => "Mage",
        SpecialAdventurer special => $"Special ({special.GetType().Name})",
        _ => "Unknown"
    };

    // Parses recruit numbers from input, accepting both commas and spaces.
    // FR : Analyse les numéros de recrues, en acceptant virgules ET espaces.
    private static List<Adventurer> ParseChosenRecruits(string input, Guild guild)
    {
        var indices = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(s => int.TryParse(s, out int i) && i >= 0 && i < guild.Roster.Count)
            .Select(int.Parse)
            .Distinct();

        return indices.Select(i => guild.Roster[i]).ToList();
    }

    // Shows the quest's flavor text (objective + intro line in the active
    // voice), then lets the player assign recruits, previews the estimated
    // success rate, and asks for confirmation before actually sending them.
    // FR : Affiche l'objectif/l'intro de la quête, permet d'assigner des
    // recrues, prévisualise le taux de réussite estimé, puis demande
    // confirmation avant l'envoi réel.
    private static void AssignRecruitsToQuest(Quest quest, Guild guild,
        List<(int Day, Quest Quest, bool Success)> questLog, int day, Random rng)
    {
        var flavor = QuestFlavorRepository.GetRandomForType(quest.Type, rng);
        if (flavor is not null)
        {
            Console.WriteLine($"\nObjectif : {flavor.Summary}");
            Console.WriteLine(guild.Narration.CurrentVoice == NarrativeVoice.Serious
                ? flavor.SeriousIntro
                : flavor.WtfIntro);
        }

        Console.WriteLine($"\nQuête : {quest}");
        if (quest.RequiredClassName is not null)
            Console.WriteLine($"(Nécessite au moins un {quest.RequiredClassName} dans l'équipe.)");
        Console.WriteLine($"(Taille d'équipe : de {quest.MinimumTeamSize} à {quest.MaximumTeamSize} recrues.)");

        Console.WriteLine("Assignez des recrues par numéro (espaces ou virgules), ou appuyez sur Entrée pour annuler :");
        for (int i = 0; i < guild.Roster.Count; i++)
        {
            string usedTag = guild.Roster[i].UsedToday ? " (déjà en quête aujourd'hui)" : "";
            string injuredTag = guild.Roster[i].IsInjured ? " (blessé, malus 15%)" : "";
            Console.WriteLine($"  {i}: {guild.Roster[i]} [{DescribeType(guild.Roster[i])}]{usedTag}{injuredTag}");
        }

        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Annulé.");
            return;
        }

        var chosen = ParseChosenRecruits(input, guild);
        if (chosen.Count == 0)
        {
            Console.WriteLine("Aucun numéro de recrue valide reconnu - quête non tentée.");
            return;
        }

        string? requirementError = quest.DescribeUnmetRequirement(chosen);
        if (requirementError is not null)
        {
            Console.WriteLine($"Impossible d'envoyer cette équipe : {requirementError}");
            return;
        }

        var alreadyUsedRecruits = chosen.Where(r => r.UsedToday).ToList();
        if (alreadyUsedRecruits.Count > 0)
        {
            string names = string.Join(", ", alreadyUsedRecruits.Select(r => r.Name));
            Console.WriteLine($"Impossible d'envoyer cette équipe : {names} est/sont déjà parti(s) en quête aujourd'hui. " +
                               "Changez d'équipe ou recrutez de nouvelles recrues.");
            return;
        }

        int previewRate = QuestResolver.PreviewSuccessRate(quest, chosen, guild.Resources.Food);
        Console.WriteLine($"\nTaux de réussite estimé avec cette équipe : {previewRate}%");
        Console.Write("Envoyer l'équipe sur la quête ? (o/n) : ");
        if (Console.ReadLine()?.Trim().ToLower() != "o")
        {
            Console.WriteLine("Assignation annulée - la quête reste disponible.");
            return;
        }

        bool success = guild.AttemptQuest(quest, chosen);
        questLog.Add((day, quest, success));

        if (success)
        {
            Console.WriteLine($"-> {quest.Name} RÉUSSIE. +{quest.GoldReward} or.");
        }
        else if (previewRate < Guild.DeathThreshold)
        {
            var dead = chosen.Where(r => !guild.Roster.Contains(r)).Select(r => r.Name).ToList();
            var survivors = chosen.Where(r => guild.Roster.Contains(r)).Select(r => r.Name).ToList();

            Console.WriteLine($"-> {quest.Name} ÉCHOUÉE. Le taux estimé ({previewRate}%) était sous les {Guild.DeathThreshold}%.");
            if (dead.Count > 0)
                Console.WriteLine($"   {string.Join(", ", dead)} ne reviennent pas de cette quête.");
            if (survivors.Count > 0)
                Console.WriteLine($"   {string.Join(", ", survivors)} (personnage spécial, insensible à la mort) reviennent blessés.");
        }
        else
        {
            string names = string.Join(", ", chosen.Select(r => r.Name));
            Console.WriteLine($"-> {quest.Name} ÉCHOUÉE. L'équipe revient blessée ({names}) - " +
                               "soignez-la avec une potion (option 7) ou elle gardera un malus de 15% à sa prochaine quête.");
        }
    }

    // Creates a fresh Guild with the starting roster (new game).
    // FR : Crée une nouvelle Guild avec le roster de départ (nouvelle partie).
    private static Guild CreateNewGuild()
    {
        var guild = new Guild();
        guild.RecruitAdventurer(new Sameth());
        guild.RecruitAdventurer(new Meloap());
        guild.RecruitAdventurer(new Elowen());
        guild.RecruitAdventurer(new Warrior("Bram"));
        guild.RecruitAdventurer(new Healer("Elara"));
        guild.RecruitAdventurer(new Mage("Yssa"));
        return guild;
    }

    public static void Run()
    {
        Console.WriteLine("=== GUILD MANAGER - TMP terminal loop (build: no-shop-class-v11) ===\n");
        Console.WriteLine("1: Start a new game");
        Console.WriteLine("2: Load saved game");
        Console.Write("> ");

        Guild guild;
        if (Console.ReadLine()?.Trim() == "2")
        {
            try
            {
                guild = SaveManager.Load(SavePath);
                Console.WriteLine("Save loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load save ({ex.Message}). Starting a new game instead.");
                guild = CreateNewGuild();
            }
        }
        else
        {
            guild = CreateNewGuild();
        }

        // IMPORTANT: use guild.Narration everywhere below, never a separate
        // NarrativeManager - otherwise Save/Load silently loses the active
        // voice and the switch flag, since SaveManager reads guild.Narration.
        // FR : IMPORTANT — toujours utiliser guild.Narration, jamais une
        // instance séparée, sinon la sauvegarde perd la voix active.
        guild.Narration.OnSwitchToWtf += npc =>
            Console.WriteLine($"\n>>> Bascule définitive vers la Voix WTF (quête de {npc.Name} acceptée). <<<\n");

        var rng = new Random();
        var questLog = new List<(int Day, Quest Quest, bool Success)>();

        while (!guild.Cycle.IsGameOver)
        {
            int day = guild.Cycle.CurrentDay;
            int questLevel = QuestGenerator.GetQuestLevelForDay(day);
            Console.WriteLine($"\n----- Jour {day} / 10 (niveau de quête : {questLevel}) -----");
            Console.WriteLine($"Or : {guild.Resources.Gold} | Nourriture : {guild.Resources.Food} | " +
                               $"Potions : {guild.Resources.HealthPotions} | Voix : {guild.Narration.CurrentVoice}");

            // --- Any dialogue scheduled for today ---
            foreach (var entry in DialogueRepository.ForDay(day))
                Console.WriteLine($"[{entry.Character}] {guild.Narration.GetText(entry)}");

            // --- Improbable NPC encounter: point of no return (GDD 4.2) ---
            var npc = NpcForDay(day);
            if (npc is not null && !guild.Narration.SwitchAlreadyTriggered)
            {
                Console.WriteLine($"\nUn PNJ improbable approche : {npc.Name}");
                Console.WriteLine(npc.Appearance);
                Console.WriteLine($"Quête proposée : {npc.QuestHook}");
                Console.Write("Accepter la quête ? (o/n) : ");

                if (Console.ReadLine()?.Trim().ToLower() == "o")
                    guild.Narration.AcceptImprobableNpcQuest(npc);
                else
                    guild.Narration.DeclineImprobableNpcQuest();
            }

            // --- Daily upkeep ---
            bool fed = guild.ApplyDailyUpkeep();
            if (!fed)
                Console.WriteLine("ATTENTION : pas assez de nourriture pour nourrir tout le roster aujourd'hui.");

            // --- Planning phase menu: repeat actions until the player ends the day ---
            // FR : Menu de planification : actions répétables jusqu'à ce que le joueur termine la journée.
            var todaysQuests = QuestGenerator.GenerateForDay(day);
            bool dayEnded = false;

            while (!dayEnded)
            {
                Console.WriteLine("\nPhase de planification - choisissez une action :");
                Console.WriteLine("  1 : Voir les quêtes disponibles et en choisir une");
                Console.WriteLine("  2 : Recruter un nouvel aventurier");
                Console.WriteLine("  3 : Boutique (nourriture / potions de soin)");
                Console.WriteLine("  4 : Voir le roster");
                Console.WriteLine("  5 : Voir le journal des quêtes (cette partie)");
                Console.WriteLine("  6 : Sauvegarder la partie");
                Console.WriteLine("  7 : Soigner une recrue avec une potion");
                Console.WriteLine("  8 : Terminer la journée (passer à la résolution)");
                Console.Write("> ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        var pending = todaysQuests.Where(q => !q.IsResolved).ToList();
                        if (pending.Count == 0)
                        {
                            Console.WriteLine("Plus aucune quête à assigner aujourd'hui.");
                            break;
                        }

                        Console.WriteLine("\nQuêtes disponibles aujourd'hui :");
                        for (int i = 0; i < pending.Count; i++)
                            Console.WriteLine($"  {i}: {pending[i]}");

                        Console.Write("Choisissez un numéro de quête, ou appuyez sur Entrée pour revenir : ");
                        string? questChoice = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(questChoice))
                            break;

                        if (int.TryParse(questChoice.Trim(), out int questIndex) &&
                            questIndex >= 0 && questIndex < pending.Count)
                        {
                            AssignRecruitsToQuest(pending[questIndex], guild, questLog, day, rng);
                        }
                        else
                        {
                            Console.WriteLine("Numéro de quête invalide.");
                        }
                        break;

                    case "2":
                        string candidateName = guild.NamePool.DrawUniqueName(rng);
                        var candidate = AdventurerFactory.GenerateRandomRecruit(candidateName, rng);
                        Console.WriteLine($"\nUn candidat se présente : {candidate} [{DescribeType(candidate)}] - recrutement : {HireCost} or.");
                        Console.Write("Le recruter ? (o/n) : ");
                        if (Console.ReadLine()?.Trim().ToLower() == "o")
                        {
                            if (guild.Resources.SpendGold(HireCost))
                            {
                                guild.RecruitAdventurer(candidate);
                                Console.WriteLine($"{candidate.Name} a rejoint la guilde.");
                            }
                            else
                            {
                                Console.WriteLine($"Pas assez d'or pour recruter (besoin de {HireCost}, disponible {guild.Resources.Gold}).");
                            }
                        }
                        break;

                    case "3":
                        Console.WriteLine("\nBoutique de Sameth :");
                        Console.WriteLine($"  1: Nourriture ({GoldPerFoodUnit} or/unité)");
                        Console.WriteLine($"  2: Potions de soin ({GoldPerHealthPotion} or/unité)");
                        Console.Write($"> (vous avez {guild.Resources.Gold} or) ");
                        string? shopChoice = Console.ReadLine();

                        if (shopChoice?.Trim() == "1")
                        {
                            Console.Write("Combien de nourriture acheter ? ");
                            if (int.TryParse(Console.ReadLine(), out int foodAmount) && foodAmount > 0)
                            {
                                int cost = foodAmount * GoldPerFoodUnit;
                                if (guild.Resources.SpendGold(cost))
                                {
                                    guild.Resources.AddFood(foodAmount);
                                    Console.WriteLine($"{foodAmount} nourriture achetée pour {cost} or.");
                                }
                                else
                                {
                                    Console.WriteLine($"Pas assez d'or (besoin de {cost}, disponible {guild.Resources.Gold}).");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Montant invalide - rien n'a été acheté.");
                            }
                        }
                        else if (shopChoice?.Trim() == "2")
                        {
                            Console.Write("Combien de potions de soin acheter ? ");
                            if (int.TryParse(Console.ReadLine(), out int potionAmount) && potionAmount > 0)
                            {
                                int cost = potionAmount * GoldPerHealthPotion;
                                if (guild.Resources.SpendGold(cost))
                                {
                                    guild.Resources.AddHealthPotions(potionAmount);
                                    Console.WriteLine($"{potionAmount} potion(s) de soin achetée(s) pour {cost} or.");
                                }
                                else
                                {
                                    Console.WriteLine($"Pas assez d'or (besoin de {cost}, disponible {guild.Resources.Gold}).");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Montant invalide - rien n'a été acheté.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Choix non reconnu - rien n'a été acheté.");
                        }
                        break;

                    case "4":
                        Console.WriteLine("\nRoster actuel :");
                        foreach (var recruit in guild.Roster)
                        {
                            string usedTag = recruit.UsedToday ? " (déjà en quête aujourd'hui)" : "";
                            string injuredTag = recruit.IsInjured ? " (blessé, malus 15%)" : "";
                            Console.WriteLine($"  {recruit} [{DescribeType(recruit)}]{usedTag}{injuredTag}");
                        }
                        break;

                    case "5":
                        Console.WriteLine("\nJournal des quêtes :");
                        if (questLog.Count == 0)
                            Console.WriteLine("  (aucune quête tentée pour l'instant)");
                        foreach (var (logDay, quest, success) in questLog)
                            Console.WriteLine($"  Jour {logDay} : {quest.Name} - {(success ? "RÉUSSITE" : "ÉCHEC")}");
                        break;

                    case "6":
                        SaveManager.Save(guild, SavePath);
                        Console.WriteLine($"Partie sauvegardée dans {SavePath}.");
                        break;

                    case "7":
                        var injuredOrHealable = guild.Roster.Where(r => r.IsInjured).ToList();
                        if (injuredOrHealable.Count == 0)
                        {
                            Console.WriteLine("Personne n'est blessé pour l'instant.");
                            break;
                        }

                        Console.WriteLine($"\nPotions en stock : {guild.Resources.HealthPotions}");
                        Console.WriteLine("Recrues blessées :");
                        for (int i = 0; i < injuredOrHealable.Count; i++)
                            Console.WriteLine($"  {i}: {injuredOrHealable[i]} [{DescribeType(injuredOrHealable[i])}]");

                        Console.Write("Numéro de la recrue à soigner, ou Entrée pour annuler : ");
                        string? healChoice = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(healChoice))
                            break;

                        if (int.TryParse(healChoice.Trim(), out int healIndex) &&
                            healIndex >= 0 && healIndex < injuredOrHealable.Count)
                        {
                            bool healed = guild.UseHealthPotion(injuredOrHealable[healIndex]);
                            Console.WriteLine(healed
                                ? $"{injuredOrHealable[healIndex].Name} est soigné(e) - malus levé."
                                : "Plus aucune potion en stock.");
                        }
                        else
                        {
                            Console.WriteLine("Numéro invalide.");
                        }
                        break;

                    case "8":
                        dayEnded = true;
                        break;

                    default:
                        Console.WriteLine("Choix non reconnu.");
                        break;
                }
            }

            // --- Resolution done, move to the next day ---
            if (!guild.Cycle.IsGameOver)
            {
                guild.Cycle.AdvanceToNextDay();
                guild.ResetDailyUsage();
            }
            else
            {
                break; // day 10 just finished, stop before AdvanceToNextDay throws
            }
        }

        // --- TMP ending: real ending text should live in dialogues.json eventually ---
        // FR : TMP — le vrai texte de fin devra vivre dans dialogues.json.
        Console.WriteLine("\n=== FIN DU JOUR 10 ===");
        bool guildSaved = guild.Resources.ReachedFinalGoal();

        if (guildSaved)
        {
            Console.WriteLine("La guilde a atteint 5000 or. Elle survit.");
        }
        else if (guild.Narration.CurrentVoice == NarrativeVoice.Serious)
        {
            Console.WriteLine("La guilde ferme. Vous vous retrouvez seul au comptoir d'un bar miteux.");
        }
        else
        {
            Console.WriteLine("La guilde ferme. Vous vous retrouvez en pleine conversation avec un groupe d'animaux.");
        }
    }
}