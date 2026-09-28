using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.Recrutments;

public partial class RecrutementsView : UserControl
{
    // Recruitment candidates are always generic (Warrior/Healer/Mage), never
    // special adventurers - same sheets/grid as GuildMembersView.
    private static readonly Dictionary<string, (string File, int Cols, int Rows)> PortraitSheets = new()
    {
        ["Warrior"] = ("Guerrier(sprites).png", 5, 3),
        ["Healer"] = ("Healer(sprites).png", 5, 3),
        ["Mage"] = ("Mage_Noir(sprites).png", 5, 3),
    };

    private static readonly Dictionary<string, CroppedBitmap> _portraitCache = new();

    public RecrutementsView()
    {
        InitializeComponent();
        RefreshCandidates();
    }

    private static CroppedBitmap? LoadPortrait(Adventurer recruit)
    {
        string key = recruit.GetType().Name;
        if (!PortraitSheets.TryGetValue(key, out var info))
            return null;

        if (_portraitCache.TryGetValue(key, out var cached))
            return cached;

        var uri = new Uri($"pack://application:,,,/GuildManager;component/Assets/{info.File}");
        var fullSheet = new BitmapImage(uri);

        int cellWidth = fullSheet.PixelWidth / info.Cols;
        int cellHeight = fullSheet.PixelHeight / info.Rows;

        var cropped = new CroppedBitmap(fullSheet, new Int32Rect(0, 0, cellWidth, cellHeight));
        _portraitCache[key] = cropped;
        return cropped;
    }

    // Shows today's 6 recruitment candidates (name/class/level + portrait).
    // A slot beyond the current candidate count is left blank.
    private void RefreshCandidates()
    {
        var guild = GameSessionManager.Current;
        var candidates = GameSessionManager.TodaysRecruitCandidates;
        int cost = guild is not null ? RecruitmentCost(guild.Cycle.CurrentDay) : 0;
        var textSlots = new[] { Unit1Text, Unit2Text, Unit3Text, Unit4Text, Unit5Text, Unit6Text };
        var portraitSlots = new[] { Unit1Portrait, Unit2Portrait, Unit3Portrait, Unit4Portrait, Unit5Portrait, Unit6Portrait };

        for (int i = 0; i < textSlots.Length; i++)
        {
            if (i < candidates.Count)
            {
                var candidate = candidates[i];
                textSlots[i].Text = $"{candidate.Name}\n{candidate.GetType().Name} — Niv. {candidate.Level}\n💰 {cost} or";
                portraitSlots[i].Source = LoadPortrait(candidate);
                portraitSlots[i].Visibility = Visibility.Visible;
            }
            else
            {
                textSlots[i].Text = "";
                portraitSlots[i].Source = null;
                portraitSlots[i].Visibility = Visibility.Collapsed;
            }
        }
    }

    // Recruiting a candidate costs gold, scaling with the day (same pattern
    // as quest gold rewards) so recruitment stays a meaningful choice as
    // the guild grows richer.
    private static int RecruitmentCost(int day) => 50 + day * 10;

    private void Recruit(int slotIndex)
    {
        var guild = GameSessionManager.Current;
        var candidates = GameSessionManager.TodaysRecruitCandidates;
        if (guild is null || slotIndex >= candidates.Count)
        {
            StatusText.Text = "Aucun candidat à cet emplacement.";
            return;
        }

        var recruit = candidates[slotIndex];
        int cost = RecruitmentCost(guild.Cycle.CurrentDay);

        var confirm = MessageBox.Show(
            $"Recruter {recruit.Name} ({recruit.GetType().Name}) pour {cost} or ?\nOr actuel : {guild.Resources.Gold}",
            "Recruter", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        if (!guild.Resources.SpendGold(cost))
        {
            StatusText.Text = $"Pas assez d'or pour recruter {recruit.Name} (il faut {cost}, stock : {guild.Resources.Gold}).";
            return;
        }

        guild.RecruitAdventurer(recruit);
        candidates.RemoveAt(slotIndex);

        StatusText.Text = $"{recruit.Name} a rejoint la guilde pour {cost} or !";
        RefreshCandidates();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

    private void Unit1Button_Click(object sender, RoutedEventArgs e) => Recruit(0);
    private void Unit2Button_Click(object sender, RoutedEventArgs e) => Recruit(1);
    private void Unit3Button_Click(object sender, RoutedEventArgs e) => Recruit(2);
    private void Unit4Button_Click(object sender, RoutedEventArgs e) => Recruit(3);
    private void Unit5Button_Click(object sender, RoutedEventArgs e) => Recruit(4);
    private void Unit6Button_Click(object sender, RoutedEventArgs e) => Recruit(5);
}
