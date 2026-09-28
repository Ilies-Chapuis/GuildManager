using System.Windows;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.UI.RecrutementsPageView;

public partial class RecruitmentSuccessWindow : Window
{
    public RecruitmentSuccessWindow(Adventurer recruit)
    {
        InitializeComponent();

        RecruitmentMessageText.Text = 
        $"{recruit.Name} a rejoint votre guilde ! \n\n" +
        $"Classe : {recruit.GetType().Name}\n" +
        $"Niveau : {recruit.Level}\n" +
        $"PV : {recruit.HealthPoints} / {recruit.MaxHealthPoints}";
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}