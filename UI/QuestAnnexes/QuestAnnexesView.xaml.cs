using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.Quest;
using GuildManager.UI.Dialogues;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.QuestAnnexes;

public partial class QuestAnnexesView : UserControl
{
        public QuestAnnexesView()
        {
                InitializeComponent();
        }

        private  void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
                mainWindow.ChangeMainContent(new QuestView());
        }

        // Shows today's dialogue lines (if any are scheduled for the current
        // day) after a side quest, then returns to GameView. Falls back to
        // GameView directly when nothing is scheduled today.
        private void QuestAnnexesButton_Click(object sender, RoutedEventArgs e)
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