using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_LoadGame;
using GuildManager.UI.The_MainWindow;
using GuildManager.UI.The_Menu;

namespace GuildManager.UI.The_LoginSolo;
public partial class LoginViewSolo : UserControl
{
    public LoginViewSolo()
    {
        InitializeComponent();

        // Retrieve the Storyboard (ParchmentAppear) into the parchmentAnimation variable
        Storyboard parchmentAnimation =(Storyboard)FindResource("ParchmentAppear");

        // Lancement de l'animation
        parchmentAnimation.Begin(this);
    }

    // Load la gameView
    private void NewGameButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        GameSessionManager.StartNewGame();
        MainWindow mainWindow =(MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

    private void ContinueButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        
    }

    // Load Charger
    private void LoadButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new LoadGameView());
    }

    // Handles the back button
    private void ReturnButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new MainMenuView());
    }

    
}