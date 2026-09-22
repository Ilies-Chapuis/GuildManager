using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_LoginSolo;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.The_LoadGame;

public partial class LoadGameView : UserControl
{
    public LoadGameView()
    {
        InitializeComponent();
    }

    private void Slot1Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Slot2Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Slot3Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new LoginViewSolo());
    }
}