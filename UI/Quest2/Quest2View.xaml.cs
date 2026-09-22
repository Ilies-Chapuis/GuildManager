using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.Quest;
using GuildManager.UI.The_Game;
using GuildManager.UI.Dialogues;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.Quest2;

public partial class Quest2View : UserControl
{
    public Quest2View()
    {
        InitializeComponent();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestView());
    }

    private void Quest2Button_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new DialoguesView());
    }

}