using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.Inventory;

public partial class InventoryView : UserControl
{
    public InventoryView()
    {
        InitializeComponent();
        
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

}