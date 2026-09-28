using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.Gameplay.Characters;
using GuildManager.Logic.MainMenu;

namespace GuildManager.UI.RecrutementsPageView;

public partial class UnitDetailsView : UserControl
{
    private readonly Adventurer _recruit;
    public UnitDetailsView(Adventurer recruit)
    {
        InitializeComponent();

        _recruit = recruit;

        UnitNameText.Text = recruit.Name;
        UnitDescriptionText.Text = $"Classe : {recruit.GetType().Name}";
        UnitLevelText.Text = $"Niveau : {recruit.Level}";
        UnitCostText.Text = $"PV : {recruit.HealthPoints} / {recruit.MaxHealthPoints}";
    }

    private void RecruitButton_Click(object sender, RoutedEventArgs e)
    {
        if (GameSessionManager.Current is null)
        {
            return;
        }

        var guild = GameSessionManager.Current;

        // Ajoute réellement la recrue au roster.
        guild.RecruitAdventurer(_recruit);

        // Affiche la fenêtre personnalisée.
        RecruitmentSuccessWindow successWindow = new RecruitmentSuccessWindow(_recruit);

        successWindow.Owner = Window.GetWindow(this);

        successWindow.ShowDialog();

        // Ferme la fenêtre des détails.
        Window.GetWindow(this)?.Close();
    }
}