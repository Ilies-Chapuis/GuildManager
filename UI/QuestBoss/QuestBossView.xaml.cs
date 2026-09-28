using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Quests;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.Dialogues;
using GuildManager.UI.Quest;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;
using QuestModel = GuildManager.Logic.Gameplay.Quests.Quest;

namespace GuildManager.UI.QuestBoss;

public partial class QuestBossView : UserControl
{
    private readonly List<Adventurer> _team = new();

    public QuestBossView()
    {
        InitializeComponent();

        var guild = GameSessionManager.Current;
        var boss = GameSessionManager.TodaysBossQuest;

        if (guild is null || boss is null)
        {
            // No boss fight scheduled today (shouldn't normally happen,
            // since QuestView only lets the player in here on day 5/10).
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.ChangeMainContent(new QuestView());
            return;
        }

        BossNameText.Text = $"{boss.Name} — {boss.DurationHours}h, {boss.GoldReward} or, difficulté redoutable";

        // Boss encounters open on a narrative beat before the player can
        // even pick a team - see the dialogue system for the CG scene mode.
        ShowEncounterThenTeamBuilder(guild, boss);
    }

    // ---- Narrative intro, then reveal the team-building screen ----

    private void ShowEncounterThenTeamBuilder(Guild guild, QuestModel boss)
    {
        string encounterId = boss.DayAvailable == 5 ? "nyxaria_boss_encounter" : "final_boss_showdown";
        var entry = App.Dialogues.GetById(encounterId);

        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

        if (entry is null)
        {
            RefreshLists();
            return;
        }

        var viewModel = new DialogueBoxViewModel(App.Portraits);
        var dialoguesView = new DialoguesView { DataContext = viewModel };

        viewModel.Finished += () =>
        {
            mainWindow.ChangeMainContent(this);
            RefreshLists();
        };

        mainWindow.ChangeMainContent(dialoguesView);
        viewModel.PlaySingle(entry);
    }

    // ---- Team building ----

    private void RefreshLists()
    {
        var guild = GameSessionManager.Current;
        if (guild is null) return;

        AvailableRecruitListBox.ItemsSource = guild.Roster
            .Where(r => !r.UsedToday && !_team.Contains(r))
            .ToList();

        TeamListBox.ItemsSource = _team.ToList();

        UpdateSuccessRateDisplay();
    }

    private void UpdateSuccessRateDisplay()
    {
        var guild = GameSessionManager.Current;
        var boss = GameSessionManager.TodaysBossQuest;

        if (guild is null || boss is null)
        {
            SuccessRateText.Text = "";
            return;
        }

        if (_team.Count == 0)
        {
            SuccessRateText.Text = "Ajoute des recrues à l'équipe pour voir la chance de réussite.";
            return;
        }

        string? unmet = guild.DescribeAttemptBlocker(boss, _team);
        if (unmet is not null)
        {
            SuccessRateText.Text = $"⚠ {unmet}";
            return;
        }

        int rate = QuestResolver.PreviewSuccessRate(boss, _team, guild.Resources.Food);
        SuccessRateText.Text = $"Réussite estimée : {rate}%  —  Nourriture : {boss.FoodCost} (stock {guild.Resources.Food})  —  Durée : {boss.DurationHours}h (reste {guild.Cycle.RemainingHours}h)";
    }

    private void AddToTeamButton_Click(object sender, RoutedEventArgs e)
    {
        if (AvailableRecruitListBox.SelectedItem is not Adventurer recruit) return;

        _team.Add(recruit);
        StatusText.Text = "";
        RefreshLists();
    }

    private void RemoveFromTeamButton_Click(object sender, RoutedEventArgs e)
    {
        if (TeamListBox.SelectedItem is not Adventurer recruit) return;

        _team.Remove(recruit);
        StatusText.Text = "";
        RefreshLists();
    }

    // ---- Sending the team into battle ----

    private void SendTeamButton_Click(object sender, RoutedEventArgs e)
    {
        var guild = GameSessionManager.Current;
        var boss = GameSessionManager.TodaysBossQuest;

        if (guild is null || boss is null)
        {
            StatusText.Text = "Aucun combat de boss disponible.";
            return;
        }

        string? unmet = boss.DescribeUnmetRequirement(_team);
        if (unmet is not null)
        {
            StatusText.Text = unmet;
            return;
        }

        // Covers food, hours left today, level and "already left today": a
        // refused attempt must never be reported to the player as a failed quest.
        string? blocker = guild.DescribeAttemptBlocker(boss, _team);
        if (blocker is not null)
        {
            StatusText.Text = blocker;
            return;
        }

        int rate = QuestResolver.PreviewSuccessRate(boss, _team, guild.Resources.Food);
        var names = string.Join(", ", _team.Select(a => a.Name));

        var confirm = MessageBox.Show(
            $"Envoyer {names} affronter {boss.Name} ?\nChance de réussite estimée : {rate}%",
            "Confirmer l'assaut",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.OK) return;

        bool wasSuccessful = guild.AttemptQuest(boss, _team);
        if (!boss.IsResolved)
        {
            // Safety net: the attempt was refused, so nothing happened and the
            // quest stays available.
            StatusText.Text = "L'équipe n'a pas pu partir. Réessaie.";
            return;
        }
        GameSessionManager.TodaysBossQuest = null;

        ShowOutcomeDialogueThenReturn(boss, wasSuccessful);
    }

    // ---- Outcome dialogue (defeat/taunt), then back to GameView ----

    private void ShowOutcomeDialogueThenReturn(QuestModel boss, bool wasSuccessful)
    {
        string outcomeId = boss.DayAvailable == 5
            ? (wasSuccessful ? "nyxaria_defeat" : "nyxaria_taunt")
            : (wasSuccessful ? "grendel_defeat" : "grendel_taunt");

        var entry = App.Dialogues.GetById(outcomeId);
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

        if (entry is null)
        {
            MessageBox.Show(
                wasSuccessful ? $"Victoire ! +{boss.GoldReward} or." : "Défaite... l'équipe rentre blessée (ou pire).",
                wasSuccessful ? "Succès" : "Échec",
                MessageBoxButton.OK,
                wasSuccessful ? MessageBoxImage.Information : MessageBoxImage.Warning);
            mainWindow.ChangeMainContent(new GameView());
            return;
        }

        var viewModel = new DialogueBoxViewModel(App.Portraits);
        var dialoguesView = new DialoguesView { DataContext = viewModel };
        viewModel.Finished += () => mainWindow.ChangeMainContent(new GameView());

        mainWindow.ChangeMainContent(dialoguesView);
        viewModel.PlaySingle(entry);
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestView());
    }
}
