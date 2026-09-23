using System.Windows;
using System.Windows.Controls;

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
    }
}