using System.Windows.Controls;
using GuildManager.Models;

namespace GuildManager.UI.RecrutementsPageView;

public partial class UnitDetailsView : UserControl
{
    public UnitDetailsView(Unit unit)
    {
        InitializeComponent();

        UnitNameText.Text = unit.Name;
        UnitDescriptionText.Text = unit.Description;
        UnitLevelText.Text = $"Niveau : {unit.Level}";
        UnitCostText.Text = $"Coût : {unit.Cost}";
    }
}