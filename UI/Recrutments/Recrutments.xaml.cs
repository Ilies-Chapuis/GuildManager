using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.Recrutments;

public partial class RecrutementsView : UserControl
{
    public RecrutementsView()
    {
        InitializeComponent();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

    private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void NextPageButton_Click(object sender, RoutedEventArgs e)
    {
        
    }
}