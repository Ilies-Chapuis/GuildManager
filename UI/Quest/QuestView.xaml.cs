using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.Quest2;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;
using GuildManager.UI.QuestAnnexes;
using GuildManager.UI.QuestBoss;

namespace GuildManager.UI.Quest;

public partial class QuestView : UserControl
{
    // Boss fights only exist on day 5 (Nyxaria) and day 10 (Grendel) - see
    // QuestGenerator/DialogueRepository. The poster glows on those days and
    // is otherwise a no-op.
    private static readonly int[] BossDays = { 5, 10 };

    public QuestView()
    {
        InitializeComponent();
        UpdateBossAvailability();
    }

    private bool IsBossDayToday()
    {
        var guild = GameSessionManager.Current;
        return guild is not null && System.Array.IndexOf(BossDays, guild.Cycle.CurrentDay) >= 0;
    }

    private void UpdateBossAvailability()
    {
        BossGlowBorder.Visibility = IsBossDayToday() ? Visibility.Visible : Visibility.Collapsed;
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
        if (!IsBossDayToday())
        {
            MessageBox.Show("Aucun combat de boss aujourd'hui. Reviens au jour 5 ou au jour 10.",
                "Pas de combat", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new QuestBossView());
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

}
