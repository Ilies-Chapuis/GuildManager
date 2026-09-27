using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.MainMenu;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.UI.RecrutementsPageView;

public partial class RecrutementsPage1View : UserControl
{
    public RecrutementsPage1View()
    {
        InitializeComponent();
    }
    private void Recruit(string type)
    {
        if (GameSessionManager.Current is null)
        {
            MessageBox.Show("Aucune partie en cours.",
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

    private void Unit1Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Warrior");
    }

    private void Unit2Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Healer");
    }

    private void Unit3Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Mage");
    }

    private void Unit4Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Warrior");
    }

    private void Unit5Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Healer");
    }

    private void Unit6Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Mage");
    }

    private void Unit7Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Warrior");
    }

    private void Unit8Button_Click(object sender, RoutedEventArgs e)
    {
        Recruit("Mage");
    }
}