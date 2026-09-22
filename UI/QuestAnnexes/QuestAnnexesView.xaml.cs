using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.Quest;
using GuildManager.UI.Dialogues;
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

        private void QuestAnnexesButton_Click(object sender, RoutedEventArgs e)
        {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
                mainWindow.ChangeMainContent(new DialoguesView());
        }
}