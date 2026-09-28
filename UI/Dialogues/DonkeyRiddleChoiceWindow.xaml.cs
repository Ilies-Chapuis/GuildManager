using System.Windows;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Narrative;

namespace GuildManager.UI.Dialogues;

public partial class DonkeyRiddleChoiceWindow : Window
{
    private readonly Guild _guild;

    public DonkeyRiddleChoiceWindow(Guild guild)
    {
        InitializeComponent();
        _guild = guild;

        GoldButton.Content = BlackMarketTrade.GoldAnswerText;
        FoodButton.Content = BlackMarketTrade.FoodAnswerText;
        WrongButton.Content = BlackMarketTrade.WrongAnswerText;
    }

    private void Answer(DonkeyRiddleAnswer answer)
    {
        var result = BlackMarketTrade.Apply(_guild, answer);
        MessageBox.Show(result.FlavorText, "L'Âne s'en va...", MessageBoxButton.OK,
            result.WasTricked ? MessageBoxImage.Warning : MessageBoxImage.Information);
        Close();
    }

    private void GoldButton_Click(object sender, RoutedEventArgs e) => Answer(DonkeyRiddleAnswer.GoldAnswer);
    private void FoodButton_Click(object sender, RoutedEventArgs e) => Answer(DonkeyRiddleAnswer.FoodAnswer);
    private void WrongButton_Click(object sender, RoutedEventArgs e) => Answer(DonkeyRiddleAnswer.WrongAnswer);
}
