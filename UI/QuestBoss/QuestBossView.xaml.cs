using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.Quest;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.QuestBoss;

public partial class QuestBossView : UserControl
{
    public QuestBossView()
    {
        InitializeComponent();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestView());
    }
}