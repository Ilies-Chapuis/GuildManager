using System.Data.Common;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Models;

namespace GuildManager.UI.RecrutementsPageView;

public partial class RecrutementsPage2View : UserControl
{
    public RecrutementsPage2View()
    {
        InitializeComponent();
    }

    private void Unit9Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 9,
            Name = "Unité 9",
            Description = "Description de l'unité 9.",
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

    private void Unit10Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 10,
            Name = "Unité 10",
            Description = "Description de l'unité 10.",
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

    private void Unit11Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 10,
            Name = "Unité 11",
            Description = "Description de l'unité 11.",
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

    private void Unit12Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 12,
            Name = "Unité 12",
            Description = "Description de l'unité 12.",
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

    private void Unit13Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 13,
            Name = "Unité 13",
            Description = "Description de l'unité 13.",
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

    private void Unit14Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 14,
            Name = "Unité 14",
            Description = "Description de l'unité 14.",
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

    private void Unit15Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 15,
            Name = "Unité 15",
            Description = "Description de l'unité 15",
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

    private void Unit16Button_Click(object sender, RoutedEventArgs e)
    {
        Unit unit = new Unit
        {
            Id = 16,
            Name = "Unité 16",
            Description = "Description de l'unité 16",
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