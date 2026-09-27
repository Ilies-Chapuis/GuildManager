using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.Quest;
using GuildManager.UI.Dialogues;
using GuildManager.UI.The_Game;
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

    // Shows today's dialogue lines (if any are scheduled for the current
    // day) after a boss fight, then returns to GameView. Boss days (5, 10)
    // use the CG scene mode automatically via portraits.json's "Scenes"
    // block (Nyxaria / Grendel), no extra wiring needed here.
    private void QuestBossButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        var guild = GameSessionManager.Current;
        var todaysLines = guild is not null ? App.Dialogues.GetByDay(guild.Cycle.CurrentDay) : null;

        if (todaysLines is null || todaysLines.Count == 0)
        {
            mainWindow.ChangeMainContent(new GameView());
            return;
        }

        var viewModel = new DialogueBoxViewModel(App.Portraits);
        var dialoguesView = new DialoguesView { DataContext = viewModel };
        viewModel.Finished += () => mainWindow.ChangeMainContent(new GameView());

        mainWindow.ChangeMainContent(dialoguesView);
        viewModel.Play(todaysLines);
    }

}