using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GuildManager;

/// <summary>
/// Interaction logic MainMenuView.xaml for .xaml
/// </summary>
public partial class MainMenuView : UserControl
{
    public MainMenuView()
    {
        InitializeComponent();
    }
    public void ChangeMainContent(MainMenuView newContent)
    {
        Content = newContent;
    }
    private void QuitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}