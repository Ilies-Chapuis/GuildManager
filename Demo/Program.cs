using System;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;

var narrativeManager = new NarrativeManager();

// Le basculement est un événement one-shot : on l'écoute pour réagir à l'instant précis
// où la partie bascule définitivement (GDD 4.2).
narrativeManager.OnBasculeVersWtf += pnj =>
    Console.WriteLine($"\n>>> Vous avez accepté la quête de {pnj.Nom}. " +
                       "Bascule définitive vers la Voix WTF pour le reste de la partie. <<<\n");
