using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_Menu;

namespace GuildManager.UI.The_MainWindow;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Shows the main menu on startup
        ChangeMainContent(new MainMenuView());
    }

    public void ChangeMainContent(UserControl newContent)
    {
        MainContent.Content = newContent;
    }

    private void QuitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
