using System.Windows;
using System.Windows.Controls;
using GuildManager.Models;

namespace GuildManager.UI.RecrutementsPageView;

public partial class RecrutementsPage1View : UserControl
{
    public RecrutementsPage1View()
    {
        InitializeComponent();
    }

    private void Unit1Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id= 1,
            Name = "Unité 1",
            Description = "Description de l'unité 1.",
            Level = 1,
            Cost = 100
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width = 750,
            Height= 500
        };

        window.ShowDialog();
    }

    private void Unit2Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 2,
            Name = "Unité 2",
            Description = "Description de l'unité 2.",
            Level = 1,
            Cost = 200
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width = 750,
            Height = 500
        };

        window.ShowDialog();
    }

    private void Unit3Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 3,
            Name = "Unité 3",
            Description="Description de l'unité 3.",
            Level = 1,
            Cost = 300
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width = 750,
            Height = 500
        };

        window.ShowDialog();
    }

    private void Unit4Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 4,
            Name = "Unité 4",
            Description = "Description de l'unité 4.",
            Level = 1,
            Cost=400
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width = 750,
            Height = 500
        };

        window.ShowDialog();
    }

    private void Unit5Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 5,
            Name = "Unité 5",
            Description = "Description de l'unité 5.",
            Level = 1,
            Cost = 200
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width = 750,
            Height = 500
        };

        window.ShowDialog();
    }

    private void Unit6Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 6,
            Name = "Unité 6",
            Description = "Description de l'unité 6.",
            Level = 1,
            Cost = 200
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width= 750,
            Height = 500
        };

        window.ShowDialog();
    }

    private void Unit7Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 7,
            Name = "Unité 7",
            Description = "Description de l'unité 7.",
            Level = 1,
            Cost = 200
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width = 750,
            Height = 500
        };

        window.ShowDialog();
    }

    private void Unit8Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 8,
            Name = "Unité 8",
            Description = "Description de l'unité 8.",
            Level = 1,
            Cost = 200
        };

        UnitDetailsView detailsView = new UnitDetailsView(unit);

        Window window = new Window
        {
            Title = unit.Name,
            Content = detailsView,
            Width = 750,
            Height = 500
        };

        window.ShowDialog();
    }
}