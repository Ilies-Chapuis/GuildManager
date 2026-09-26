using System.Windows.Controls;

namespace GuildManager.UI.Pages_Navigation;

public class PageNavigator
{
    private readonly UserControl[] _pages;

    private int _currentPage;

    public PageNavigator(UserControl[] pages)
    {
        _pages = pages;

        // La première page affichée est la page 0
        _currentPage = 0;
    }

    // Passe à la page suivante
    public void NextPage()
    {
        if (_currentPage < _pages.Length - 1)
        {
            _currentPage++;
        }
    }

    // Retourne à la page précédente
    public void PreviousPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
        }
    }

    // Récupère la page actuellement sélectionnée
    public UserControl GetCurrentPage()
    {
        return _pages[_currentPage];
    }

    // Numéro de la page actuelle pour l'affichage
    public int CurrentPageNumber => _currentPage + 1;

    // Nombre total de pages
    public int TotalPages => _pages.Length;
}