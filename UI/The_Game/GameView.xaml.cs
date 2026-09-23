using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.Inventory;
using GuildManager.UI.Quest;
using GuildManager.UI.Recrutments;
using GuildManager.UI.Options;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.The_Game;

public partial class GameView : UserControl
{
    public GameView()
    {
        InitializeComponent();
        
    }

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
        
    }
}