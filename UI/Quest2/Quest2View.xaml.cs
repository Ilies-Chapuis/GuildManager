using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.Gameplay.Narrative;
using GuildManager.Logic.Gameplay.Quests;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.Quest;
using GuildManager.UI.The_MainWindow;
using QuestModel = GuildManager.Logic.Gameplay.Quests.Quest;

namespace GuildManager.UI.Quest2;

public partial class Quest2View : UserControl
{
    private readonly List<Adventurer> _team = new();
    private readonly Dictionary<QuestModel, QuestFlavorEntry> _flavorCache = new();
    private static readonly Random Rng = new();

    public Quest2View()
    {
        InitializeComponent();
        RefreshLists();
    }

    // ---- Loading / refreshing the 3 columns ----

    private void RefreshLists()
    {
        var guild = GameSessionManager.Current;
        if (guild is null)
        {
            StatusText.Text = "Aucune partie en cours.";
            return;
        }

        int selectedIndex = QuestListBox.SelectedIndex;
        QuestListBox.ItemsSource = GameSessionManager.TodaysQuests;
        if (GameSessionManager.TodaysQuests.Count > 0)
            QuestListBox.SelectedIndex = selectedIndex >= 0 && selectedIndex < GameSessionManager.TodaysQuests.Count ? selectedIndex : 0;

        // Recruits still available today = not already used, and not
        // already set aside in the team currently being assembled.
        AvailableRecruitListBox.ItemsSource = guild.Roster
            .Where(r => !r.UsedToday && !_team.Contains(r))
            .ToList();

        TeamListBox.ItemsSource = _team.ToList();

        UpdateDescription();
        UpdateSuccessRateDisplay();
    }

    private QuestModel? SelectedQuest => QuestListBox.SelectedItem as QuestModel;

    // Picks (once per quest instance, then remembers it) a random lore
    // flavor entry matching this quest's type, so the description stays
    // the same across clicks instead of re-rolling every time.
    private QuestFlavorEntry? GetOrPickFlavor(QuestModel quest)
    {
        if (_flavorCache.TryGetValue(quest, out var cached))
            return cached;

        var flavor = QuestFlavorRepository.GetRandomForType(quest.Type, Rng);
        if (flavor is not null)
            _flavorCache[quest] = flavor;
        return flavor;
    }

    private void UpdateDescription()
    {
        var quest = SelectedQuest;
        var guild = GameSessionManager.Current;

        if (quest is null || guild is null)
        {
            DescriptionText.Text = "";
            return;
        }

        var flavor = GetOrPickFlavor(quest);
        if (flavor is null)
        {
            DescriptionText.Text = "";
            return;
        }

        string intro = guild.Narration.CurrentVoice == NarrativeVoice.Wtf ? flavor.WtfIntro : flavor.SeriousIntro;
        DescriptionText.Text = $"{flavor.Summary}\n\n{intro}";
    }

    private void UpdateSuccessRateDisplay()
    {
        var quest = SelectedQuest;
        var guild = GameSessionManager.Current;

        if (quest is null || guild is null)
        {
            SuccessRateText.Text = "";
            return;
        }

        if (_team.Count == 0)
        {
            SuccessRateText.Text = "Ajoute au moins une recrue à l'équipe pour voir la chance de réussite.";
            return;
        }

        string? unmet = quest.DescribeUnmetRequirement(_team);
        if (unmet is not null)
        {
            SuccessRateText.Text = $"⚠ {unmet}";
            return;
        }

        int rate = QuestResolver.PreviewSuccessRate(quest, _team, guild.Resources.Food);
        SuccessRateText.Text = $"Chance de réussite estimée : {rate}%  —  Coût en nourriture : {quest.FoodCost} (stock : {guild.Resources.Food})";
    }

    // ---- Building the team ----

    private void QuestListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        StatusText.Text = "";
        UpdateDescription();
        UpdateSuccessRateDisplay();
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

    // ---- Sending the team ----

    private void SendTeamButton_Click(object sender, RoutedEventArgs e)
    {
        var guild = GameSessionManager.Current;
        var quest = SelectedQuest;

        if (guild is null || quest is null)
        {
            StatusText.Text = "Choisis une quête avant d'envoyer une équipe.";
            return;
        }
        if (_team.Count == 0)
        {
            StatusText.Text = "Ajoute au moins une recrue à l'équipe avant d'envoyer.";
            return;
        }

        string? unmet = quest.DescribeUnmetRequirement(_team);
        if (unmet is not null)
        {
            StatusText.Text = unmet;
            return;
        }

        if (guild.Resources.Food < quest.FoodCost)
        {
            StatusText.Text = $"Pas assez de nourriture pour approvisionner l'équipe (besoin de {quest.FoodCost}, stock : {guild.Resources.Food}).";
            return;
        }

        int rate = QuestResolver.PreviewSuccessRate(quest, _team, guild.Resources.Food);
        var names = string.Join(", ", _team.Select(a => a.Name));

        var confirm = MessageBox.Show(
            $"Envoyer {names} sur « {quest.Name} » ?\nChance de réussite estimée : {rate}%",
            "Confirmer l'envoi",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.OK) return;

        bool wasSuccessful = guild.AttemptQuest(quest, _team);

        // The quest is consumed for the day, whether it succeeds or fails.
        GameSessionManager.TodaysQuests.Remove(quest);

        MessageBox.Show(
            wasSuccessful
                ? $"Quête réussie ! +{quest.GoldReward} or."
                : "Quête échouée... l'équipe rentre blessée (ou pire, pour les recrues non spéciales).",
            wasSuccessful ? "Succès" : "Échec",
            MessageBoxButton.OK,
            wasSuccessful ? MessageBoxImage.Information : MessageBoxImage.Warning);

        _team.Clear();
        QuestListBox.SelectedIndex = -1;
        StatusText.Text = "";
        RefreshLists();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestView());
    }
}
