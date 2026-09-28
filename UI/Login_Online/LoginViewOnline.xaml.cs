using System;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.API;
using GuildManager.UI.The_MainWindow;
using GuildManager.UI.The_Menu;

namespace GuildManager.UI.Login_Online;

public partial class LoginViewOnline : UserControl
{
    // Shown after a successful login: only the account system is wired to
    // the API so far, not the online gameplay screens.
    private const string AuthOnlyNote =
        "Seule l'authentification est branchée à l'API pour l'instant (compte enregistré dans PostgreSQL).";

    public LoginViewOnline()
    {
        InitializeComponent();
    }

    // Reads and validates the two fields. Shows a message and returns false
    // when one of them is empty.
    private bool TryReadCredentials(out string pseudo, out string password)
    {
        pseudo = PseudoTextBox.Text.Trim();
        password = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(pseudo))
        {
            MessageBox.Show("Veuillez entrer un pseudo.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Veuillez entrer un mot de passe.");
            return false;
        }

        return true;
    }

    // Logs in through POST /auth/login (password checked against the BCrypt
    // hash stored in the players table).
    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadCredentials(out string pseudo, out string password)) return;

        try
        {
            var player = await OnlineSession.LoginAsync(pseudo, password);
            MessageBox.Show(
                $"Connexion réussie : {player.Username} (compte n°{player.Id}).\n\n{AuthOnlyNote}",
                "Mode en ligne");
        }
        catch (Exception ex)
        {
            MessageBox.Show(OnlineErrors.Describe(ex), "Mode en ligne", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // Creates the account through POST /auth/register (a new row in the
    // players table), then logs in with it.
    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadCredentials(out string pseudo, out string password)) return;

        try
        {
            var player = await OnlineSession.RegisterAsync(pseudo, password);
            MessageBox.Show(
                $"Inscription réussie : le compte « {player.Username} » (n°{player.Id}) a été créé.\n\n{AuthOnlyNote}",
                "Mode en ligne");
        }
        catch (Exception ex)
        {
            MessageBox.Show(OnlineErrors.Describe(ex), "Mode en ligne", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new MainMenuView());
    }
}
