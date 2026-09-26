using System.Windows;
using System.Windows.Controls;
using GuildManager.UI.The_Game;
using GuildManager.UI.Pages_Navigation;
using GuildManager.UI.RecrutementsPageView;
using GuildManager.UI.The_MainWindow;

namespace GuildManager.UI.Recrutments;

public partial class RecrutementsView : UserControl
{
    private PageNavigator _pageNavigator;

    public RecrutementsView()
    {
        InitializeComponent();

        // Création du navigateur de pages et enregistrement de la page actuelle
        // new PageNavigator(new UserControl[] => se lit comme Crée un tableau de UserControl et mets RecrutementsPage1View dedans.
        // Création du navigateur avec la liste des pages qu'il devra gérer.
        _pageNavigator = new PageNavigator(new UserControl[]
        {
            new RecrutementsPage1View(),
            new RecrutementsPage2View()
        });

        // Affiche la page actuellement sélectionnée dans le ContentControl
        UpdatePage();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
        mainWindow.ChangeMainContent(new GameView());
    }

    private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
    {
        _pageNavigator.PreviousPage();
        UpdatePage();
    }

    private void NextPageButton_Click(object sender, RoutedEventArgs e)
    {
        _pageNavigator.NextPage();
        UpdatePage();
    }

    private void UpdatePage()
    {
        PageContent.Content = _pageNavigator.GetCurrentPage();
        PageNumberText.Text=
            // Met à jour l'indicateur pour afficher la page actuelle et le nombre total de pages
            $"Page {_pageNavigator.CurrentPageNumber} / {_pageNavigator.TotalPages}";
    }
}