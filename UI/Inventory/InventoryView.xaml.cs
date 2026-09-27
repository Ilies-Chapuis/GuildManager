using System.Windows;
using System.Windows.Controls;
using GuildManager.Logic.MainMenu;
using GuildManager.UI.The_Game;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.Inventory;

public partial class InventoryView : UserControl
{
    // Exchange rates. Kept as simple constants here since they're a game
    // balance choice, not tied to any other system.
    private const int GoldCostForFood = 10;
    private const int FoodGained = 5;
    private const int GoldCostForPotion = 40;

    public InventoryView()
    {
        InitializeComponent();
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        var guild = GameSessionManager.Current;
        if (guild is null)
        {
            ResourcesText.Text = "Aucune partie en cours.";
            return;
        }

        ResourcesText.Text = $"💰 {guild.Resources.Gold} or   |   🍞 {guild.Resources.Food} nourriture   |   🧪 {guild.Resources.HealthPotions} potions";
    }

    private void TradeGoldForFoodButton_Click(object sender, RoutedEventArgs e)
    {
        var guild = GameSessionManager.Current;
        if (guild is null) return;

        if (!guild.Resources.SpendGold(GoldCostForFood))
        {
            StatusText.Text = $"Pas assez d'or (il faut {GoldCostForFood}).";
            return;
        }

        guild.Resources.AddFood(FoodGained);
        StatusText.Text = $"Échangé {GoldCostForFood} or contre {FoodGained} nourriture.";
        RefreshDisplay();
    }

    private void TradeGoldForPotionButton_Click(object sender, RoutedEventArgs e)
    {
        var guild = GameSessionManager.Current;
        if (guild is null) return;

        if (!guild.Resources.SpendGold(GoldCostForPotion))
        {
            StatusText.Text = $"Pas assez d'or (il faut {GoldCostForPotion}).";
            return;
        }

        guild.Resources.AddHealthPotions(1);
        StatusText.Text = $"Échangé {GoldCostForPotion} or contre 1 potion de vie.";
        RefreshDisplay();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }
}
