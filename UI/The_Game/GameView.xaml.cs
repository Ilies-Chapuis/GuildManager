using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Narrative;
using GuildManager.Logic.Gameplay.SaveSystem;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.Dialogues;
using GuildManager.UI.Inventory;
using GuildManager.UI.Quest;
using GuildManager.UI.Recrutments;
using GuildManager.UI.Options;
using GuildManager.UI.GuildMembers;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.The_Game;

public partial class GameView : UserControl
{
    // One improbable NPC per day, days 1 to 3 - see GameSessionManager.
    private static readonly Dictionary<int, (ImprobableNpc Npc, string DialogueId)> ImprobableNpcSchedule = new()
    {
        [1] = (ImprobableNpc.BlackMarketDonkey, "donkey_quest_offer"),
        [2] = (ImprobableNpc.TalkingCat, "cat_vague_contract"),
        [3] = (ImprobableNpc.MessengerPigeon, "pigeon_escort_request"),
    };

    public GameView()
    {
        InitializeComponent();
        Loaded += GameView_Loaded;
    }

    private void GameView_Loaded(object sender, RoutedEventArgs e)
    {
        var guild = GameSessionManager.Current;
        if (guild is null) return;

        UpdateStatusBar();

        if (TryShowImprobableNpcForToday(guild)) return;

        if (guild.Cycle.CurrentDay == 4 && !GameSessionManager.DonkeyRiddleShown)
        {
            GameSessionManager.DonkeyRiddleShown = true;
            ShowDonkeyRiddle(guild);
        }
    }

    // Shows today's improbable NPC encounter if one is scheduled and hasn't
    // been shown yet. Returns true if it was shown (caller should stop
    // there, since navigation already happened).
    private bool TryShowImprobableNpcForToday(Logic.Gameplay.Guild guild)
    {
        int day = guild.Cycle.CurrentDay;
        if (GameSessionManager.ImprobableNpcDaysShown.Contains(day)) return false;
        if (!ImprobableNpcSchedule.TryGetValue(day, out var scheduled)) return false;

        GameSessionManager.ImprobableNpcDaysShown.Add(day);
        ShowImprobableNpcEncounter(guild, scheduled.Npc, scheduled.DialogueId);
        return true;
    }

    private void UpdateStatusBar()
    {
        var guild = GameSessionManager.Current;
        if (guild is null) return;

        StatusBarText.Text =
            $"Jour {guild.Cycle.CurrentDay}/10  |  💰 {guild.Resources.Gold}  |  🍞 {guild.Resources.Food}  |  " +
            $"Réputation {(guild.Resources.Reputation >= 0 ? "+" : "")}{guild.Resources.Reputation}";
    }

    // ---- Jour suivant ----

    private void NextDayButton_Click(object sender, RoutedEventArgs e)
    {
        var guild = GameSessionManager.Current;
        if (guild is null) return;

        GameSessionManager.AdvanceDay();
        UpdateStatusBar();

        var ending = guild.EvaluateEnding();
        if (ending != EndingType.None)
        {
            ShowEnding(ending);
            return;
        }

        if (TryShowImprobableNpcForToday(guild)) return;

        if (guild.Cycle.CurrentDay == 4 && !GameSessionManager.DonkeyRiddleShown)
        {
            GameSessionManager.DonkeyRiddleShown = true;
            ShowDonkeyRiddle(guild);
        }
    }

    private void ShowEnding(EndingType ending)
    {
        MessageBox.Show(
            ending == EndingType.Good
                ? "🏆 Bonne fin : la guilde a réuni assez d'or avant le jour 10 !"
                : "💀 Mauvaise fin : la dette n'a pas été remboursée à temps.",
            "Fin de partie",
            MessageBoxButton.OK,
            ending == EndingType.Good ? MessageBoxImage.Information : MessageBoxImage.Warning);
    }

    // ---- Sauvegarde ----

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var guild = GameSessionManager.Current;
        if (guild is null) return;

        var slotWindow = new SaveSlotChoiceWindow(guild) { Owner = Window.GetWindow(this) };
        slotWindow.ShowDialog();
    }

    // ---- Improbable NPC encounter (days 1 to 3) ----

    private void ShowImprobableNpcEncounter(Guild guild, ImprobableNpc npc, string dialogueId)
    {
        var entry = App.Dialogues.GetById(dialogueId);
        if (entry is null) return;

        // Captured once, before navigating away: after ChangeMainContent
        // replaces this view, "this" is detached from the visual tree and
        // Window.GetWindow(this) would return null if queried again later
        // (inside the Finished callback below).
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

        var viewModel = new DialogueBoxViewModel(App.Portraits);
        var dialoguesView = new DialoguesView { DataContext = viewModel };

        viewModel.Finished += () =>
        {
            var choiceWindow = new ImprobableNpcChoiceWindow(guild, npc) { Owner = mainWindow };
            choiceWindow.ShowDialog();

            MessageBox.Show(
                choiceWindow.Accepted
                    ? "La guilde bascule définitivement en Voix WTF pour le reste de la partie."
                    : "La guilde reste en Voix Sérieuse.",
                "Conséquence", MessageBoxButton.OK, MessageBoxImage.Information);

            mainWindow.ChangeMainContent(new GameView());
        };

        mainWindow.ChangeMainContent(dialoguesView);
        viewModel.PlaySingle(entry);
    }

    // ---- The Donkey's riddle (day 4) ----

    private void ShowDonkeyRiddle(Guild guild)
    {
        var entry = App.Dialogues.GetById("donkey_riddle_day4");
        if (entry is null) return;

        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

        var viewModel = new DialogueBoxViewModel(App.Portraits);
        var dialoguesView = new DialoguesView { DataContext = viewModel };

        viewModel.Finished += () =>
        {
            var choiceWindow = new DonkeyRiddleChoiceWindow(guild) { Owner = mainWindow };
            choiceWindow.ShowDialog();

            mainWindow.ChangeMainContent(new GameView());
        };

        mainWindow.ChangeMainContent(dialoguesView);
        viewModel.PlaySingle(entry);
    }

    // ---- Navigation ----

    private void QuetesButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestView());
    }

    private void InventaireButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new InventoryView());
    }

    private void RecrutementsButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new RecrutementsView());
    }

    private void OptionsButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new OptionView());
    }

    private void MembresGuildeButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GuildMembersView());
    }
}
