using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.GuildMembers;

public partial class GuildMembersView : UserControl
{
    public GuildMembersView()
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

    private void Unit1Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 1 sélectionnée !");
    }

    private void Unit2Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 2 sélectionnée !");
    }

    private void Unit3Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 3 sélectionnée !");
    }

    private void Unit4Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 4 sélectionnée !");
    }

    private void Unit5Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 5 sélectionnée !");
    }

    private void Unit6Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 6 sélectionnée !");
    }

    private void Unit7Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 7 sélectionnée !");
    }

    private void Unit8Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Unité 8 sélectionnée !");
    }
}