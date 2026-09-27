using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.GuildMembers;

public partial class GuildMembersView : UserControl
{
    private const int SlotsPerPage = 8;
    private int _currentPage;

    // Each recruit type's portrait sheet and its grid layout (columns x
    // rows of facial expressions). The top-left cell (0,0) is used as the
    // fixed portrait for the roster book.
    private static readonly Dictionary<string, (string File, int Cols, int Rows)> PortraitSheets = new()
    {
        ["Warrior"] = ("Guerrier(sprites).png", 5, 3),
        ["Healer"] = ("Healer(sprites).png", 5, 3),
        ["Mage"] = ("Mage_Noir(sprites).png", 5, 3),
        ["Sameth"] = ("Sameth(sprites).png", 7, 4),
        ["Meloap"] = ("Meloap(sprites).png", 6, 3),
        ["Elowen"] = ("Elowen(sprites).png", 5, 3),
    };

    // Cache so each sheet is only decoded once, not once per slot/refresh.
    private static readonly Dictionary<string, CroppedBitmap> _portraitCache = new();

    public GuildMembersView()
    {
        InitializeComponent();
        RefreshPage();
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

    // Redraws the 8 slots of the current page from the shared Guild's
    // roster. A slot beyond the roster's current size is shown empty.
    private void RefreshPage()
    {
        var roster = GameSessionManager.Current?.Roster ?? new List<Adventurer>();
        var pageMembers = roster.Skip(_currentPage * SlotsPerPage).Take(SlotsPerPage).ToList();

        var textSlots = new[] { Unit1Text, Unit2Text, Unit3Text, Unit4Text, Unit5Text, Unit6Text, Unit7Text, Unit8Text };
        var portraitSlots = new[] { Unit1Portrait, Unit2Portrait, Unit3Portrait, Unit4Portrait, Unit5Portrait, Unit6Portrait, Unit7Portrait, Unit8Portrait };

        for (int i = 0; i < textSlots.Length; i++)
        {
            if (i < pageMembers.Count)
            {
                var member = pageMembers[i];
                string injuredTag = member.IsInjured ? "  🤕 Blessé" : "";
                textSlots[i].Text = $"{member.Name}\n{member.GetType().Name} — Niv. {member.Level}\nPV {member.HealthPoints}/{member.MaxHealthPoints}{injuredTag}";
                portraitSlots[i].Source = LoadPortrait(member);
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

    private Adventurer? GetMemberAt(int slotIndex)
    {
        var roster = GameSessionManager.Current?.Roster;
        if (roster is null) return null;

        int index = _currentPage * SlotsPerPage + slotIndex;
        return index < roster.Count ? roster[index] : null;
    }

    private void ShowUnitInfo(int slotIndex)
    {
        var member = GetMemberAt(slotIndex);
        if (member is null)
        {
            MessageBox.Show("Emplacement vide.");
            return;
        }

        if (member.IsInjured)
        {
            var guild = GameSessionManager.Current;
            int potions = guild?.Resources.HealthPotions ?? 0;

            var answer = MessageBox.Show(
                $"{member.Name} est blessé(e) ({member.HealthPoints}/{member.MaxHealthPoints} PV).\n" +
                $"Utiliser une potion de vie pour soigner +15 PV ? (stock : {potions})",
                "Soigner", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer == MessageBoxResult.Yes && guild is not null)
            {
                if (guild.UseHealthPotion(member))
                {
                    MessageBox.Show($"{member.Name} soigné(e) : {member.HealthPoints}/{member.MaxHealthPoints} PV.");
                    RefreshPage();
                }
                else
                {
                    MessageBox.Show("Plus de potions de vie en stock.");
                }
            }
            return;
        }

        MessageBox.Show($"{member.Name} ({member.GetType().Name}) — Niveau {member.Level}, PV {member.HealthPoints}/{member.MaxHealthPoints}");
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

    private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            RefreshPage();
        }
    }

    private void NextPageButton_Click(object sender, RoutedEventArgs e)
    {
        int total = GameSessionManager.Current?.Roster.Count ?? 0;
        if ((_currentPage + 1) * SlotsPerPage < total)
        {
            _currentPage++;
            RefreshPage();
        }
    }

    private void Unit1Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(0);
    private void Unit2Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(1);
    private void Unit3Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(2);
    private void Unit4Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(3);
    private void Unit5Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(4);
    private void Unit6Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(5);
    private void Unit7Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(6);
    private void Unit8Button_Click(object sender, RoutedEventArgs e) => ShowUnitInfo(7);
}
