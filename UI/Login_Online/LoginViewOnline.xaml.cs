using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_MainWindow;
using GuildManager.UI.The_Menu;

namespace GuildManager.UI.Login_Online;

public partial class LoginViewOnline : UserControl
{
    public LoginViewOnline()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string pseudo = PseudoTextBox.Text;
        string motDePasse = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(pseudo))
        {
            MessageBox.Show("Veuillez entrer un pseudo.");
            return;
        }

        if (string.IsNullOrWhiteSpace(motDePasse))
        {
            MessageBox.Show("Veuillez entrer un mot de passe.");
            return;
        }

        MessageBox.Show("Les informations ont bien été saisies.");
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string pseudo = PseudoTextBox.Text;
        string motDePasse = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(pseudo) || string.IsNullOrWhiteSpace(motDePasse))
        {
            MessageBox.Show("Veuillez entrer un pseudo et un mot de passe.");
            return;
        }
        MessageBox.Show("Inscription Réussie !");
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new MainMenuView());
    }
}