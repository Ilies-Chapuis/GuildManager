using System;
using System.Windows;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.SaveSystem;

namespace GuildManager.UI.Dialogues;

public partial class SaveSlotChoiceWindow : Window
{
    private readonly Guild _guild;

    // Same slot file naming as LoadGameView, so what's saved here shows up
    // there.
    public static string SlotPath(int slot) => $"Saves/slot{slot}.json";

    public SaveSlotChoiceWindow(Guild guild)
    {
        InitializeComponent();
        _guild = guild;
    }

    private void SaveToSlot(int slot)
    {
        try
        {
            SaveManager.Save(_guild, SlotPath(slot));
            MessageBox.Show($"Partie sauvegardée dans l'emplacement {slot}.", "Sauvegarde",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Échec de la sauvegarde : {ex.Message}", "Erreur",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Close();
        }
    }

    private void Slot1Button_Click(object sender, RoutedEventArgs e) => SaveToSlot(1);
    private void Slot2Button_Click(object sender, RoutedEventArgs e) => SaveToSlot(2);
    private void Slot3Button_Click(object sender, RoutedEventArgs e) => SaveToSlot(3);
}
