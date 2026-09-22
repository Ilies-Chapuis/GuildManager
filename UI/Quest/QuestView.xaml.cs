using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.Quest2;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;
using GuildManager.UI.QuestAnnexes;
using GuildManager.UI.QuestBoss;

namespace GuildManager.UI.Quest;

public partial class QuestView : UserControl
{
    public QuestView()
    {
        InitializeComponent();
        
    }

    private void Quest2Button_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new Quest2View());
    }

    private void QuestAnnexesButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestAnnexesView());
    }

    private void QuestBossButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestBossView());
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

}