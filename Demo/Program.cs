using System;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;

var narrativeManager = new NarrativeManager();

// Le basculement est un événement one-shot : on l'écoute pour réagir à l'instant précis
// où la partie bascule définitivement (GDD 4.2).
narrativeManager.OnBasculeVersWtf += pnj =>
    Console.WriteLine($"\n>>> Vous avez accepté la quête de {pnj.Nom}. " +
                       "Bascule définitive vers la Voix WTF pour le reste de la partie. <<<\n");

var sameth = new Sameth();
var meloap = new Meloap();
var elowen = new Elowen();

Console.WriteLine("=== Jour 1 ===");
Console.WriteLine(narrativeManager.ObtenirTexte(LoreRepository.Trouver("fondation_guilde")!));

Console.WriteLine($"\nUn PNJ improbable approche : {ImprobableNpc.AneDuMarcheNoir.Nom}");
Console.WriteLine(ImprobableNpc.AneDuMarcheNoir.Apparence);
Console.WriteLine($"Quête proposée : {ImprobableNpc.AneDuMarcheNoir.AccrocheQuete}");

Console.Write("\nAccepter sa quête ? (o/n) : ");
string? reponse = Console.ReadLine();

if (reponse?.Trim().ToLower() == "o")
    narrativeManager.AccepterQuetePnjImprobable(ImprobableNpc.AneDuMarcheNoir);
else
    narrativeManager.RefuserQuetePnjImprobable();

Console.WriteLine($"\nVoix active pour la suite de la partie : {narrativeManager.VoixActuelle}");

Console.WriteLine("\n=== Jour 4 ===");
Console.WriteLine($"{sameth.Nom} : {narrativeManager.ObtenirTexteAventurier(sameth)}");
Console.WriteLine(narrativeManager.ObtenirTexte(LoreRepository.Trouver("sameth_symbole_hedge")!));

Console.WriteLine("\n=== Jour 5 ===");
Console.WriteLine($"{meloap.Nom} : {narrativeManager.ObtenirTexteAventurier(meloap)}");

Console.WriteLine("\n=== Jour 10 ===");
Console.WriteLine(narrativeManager.ObtenirTexte(LoreRepository.Trouver("bilan_jour_10")!));

// Exemple d'utilisation d'Elowen et du système de résolution de quête (GDD 6.4)
int difficulteExorcisme = 40;
Console.WriteLine($"\n{elowen.Nom} tente une quête d'exorcisme (difficulté {difficulteExorcisme}) : " +
                   $"{elowen.CalculerTauxReussite(difficulteExorcisme)}% de réussite estimée.");
