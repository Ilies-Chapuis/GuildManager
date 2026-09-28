using System.Windows.Controls;
using System.Windows.Input;
using GuildManager.UI.Dialogues;

namespace GuildManager.UI.Dialogues;

public partial class DialoguesView : UserControl
{
    private DialogueBoxViewModel? ViewModel => DataContext as DialogueBoxViewModel;

    public DialoguesView()
    {
        InitializeComponent();
        Loaded += (_, _) => Focus(); // pour recevoir KeyDown des l'affichage
    }

    private void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ViewModel?.Advance();
    }

    private void Root_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space || e.Key == Key.Enter)
        {
            ViewModel?.Advance();
            e.Handled = true;
        }
    }
}
