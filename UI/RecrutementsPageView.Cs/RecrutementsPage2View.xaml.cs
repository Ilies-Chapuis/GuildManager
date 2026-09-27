using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.MainMenu;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.UI.RecrutementsPageView;

public partial class RecrutementsPage2View : UserControl
{
    public RecrutementsPage2View()
    {
        InitializeComponent();
    }

    private void Recruit(string type)
    {
        if (GameSessionManager.Current is null)
        {
            MessageBox.Show(
                "Aucune partie en cours.",
                "Erreur",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var guild = GameSessionManager.Current;

        string name = guild.NamePool.DrawUniqueName(new Random());

        Adventurer recruit = AdventurerFactory.CreateRecruit(type, name);

        guild.RecruitAdventurer(recruit);

        UnitDetailsView detailsView = new UnitDetailsView(recruit);

        Window window = new Window
        {
            Title = recruit.Name,
            Content = detailsView,
            Width = 750,
            Height = 500
        };

        window.ShowDialog();
    }

    private void Unit9Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Warrior");
    }

    private void Unit10Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Healer");
    }

    private void Unit11Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Mage");
    }

    private void Unit12Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Warrior");
    }

    private void Unit13Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Healer");
    }

    private void Unit14Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Mage");
    }

    private void Unit15Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Warrior");
    }

    private void Unit16Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Mage");
    }
}