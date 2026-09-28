using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.Gameplay.SaveSystem;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_LoginSolo;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.The_LoadGame;

public partial class LoadGameView : UserControl
{
    public LoadGameView()
    {
        InitializeComponent();
    }

    // Same slot file naming as SaveSlotChoiceWindow, so what's saved there
    // shows up here.
    private static string SlotPath(int slot) => $"Saves/slot{slot}.json";

    private void LoadSlot(int slot)
    {
        string path = SlotPath(slot);

        if (!File.Exists(Path.Combine(AppContext.BaseDirectory, path)))
        {
            MessageBox.Show($"Aucune sauvegarde dans l'emplacement {slot}.",
                "Charger une partie", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var guild = SaveManager.Load(path);
            GameSessionManager.LoadGame(guild);

            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.ChangeMainContent(new GameView());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Échec du chargement : {ex.Message}", "Erreur",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Slot1Button_Click(object sender, RoutedEventArgs e) => LoadSlot(1);
    private void Slot2Button_Click(object sender, RoutedEventArgs e) => LoadSlot(2);
    private void Slot3Button_Click(object sender, RoutedEventArgs e) => LoadSlot(3);

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new LoginViewSolo());
    }
}
