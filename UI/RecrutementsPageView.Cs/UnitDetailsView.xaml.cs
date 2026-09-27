using System.Windows.Controls;
using GuildManager.Logic.Gameplay.Characters;

namespace GuildManager.UI.RecrutementsPageView;

public partial class UnitDetailsView : UserControl
{
    public UnitDetailsView(Adventurer recruit)
    {
        InitializeComponent();

        UnitNameText.Text = recruit.Name;
        UnitDescriptionText.Text = $"Classe : {recruit.GetType().Name}";
        UnitLevelText.Text = $"Niveau : {recruit.Level}";
        UnitCostText.Text = $"PV : {recruit.HealthPoints} / {recruit.MaxHealthPoints}";
    }
}