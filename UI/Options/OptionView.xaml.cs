using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;
using GuildManager.UI.The_Menu;

namespace GuildManager.UI.Options;

public partial class OptionView : UserControl
{
    public OptionView()
    {
        InitializeComponent();
    }

    private void MainMenuButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new MainMenuView());
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }
}