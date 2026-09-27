using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GuildManager.Logic.Dialogues;

/// <summary>
/// Grille + table d'emotions (nom -> index de case, lecture gauche->droite, haut->bas)
/// pour la spritesheet d'un personnage. Modifie Data/Portraits/portraits.json pour
/// renommer/remapper une emotion : aucun changement de code necessaire.
/// </summary>
public class CharacterPortraitConfig
{
    public string SpriteSheet { get; set; } = "";
    public int Columns { get; set; } = 1;
    public int Rows { get; set; } = 1;
    public Dictionary<string, int> Emotions { get; set; } = new();

    // Present dans le JSON pour la documentation humaine, ignore au parsing.
    [JsonPropertyName("_comment")]
    public string? Comment { get; set; }
}

/// <summary>Rectangle en pixels natifs dans une image de scene CG.</summary>
public class SceneTextRect
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

/// <summary>
/// Une scene CG plein ecran (boss) : pas de portrait decoupe, le texte se pose
/// directement sur l'illustration, dans TextRect (coordonnees natives de Image,
/// mises a l'echelle via un Viewbox cote vue).
/// </summary>
public class SceneConfig
{
    public string Image { get; set; } = "";
    public double CanvasWidth { get; set; }
    public double CanvasHeight { get; set; }

    /// <summary>true si la boite est deja dessinee dans l'illustration (ex: Boss_Finale.png).</summary>
    public bool HasBakedBox { get; set; }

    public SceneTextRect TextRect { get; set; } = new();
}

/// <summary>Racine du fichier portraits.json.</summary>
public class PortraitLibraryData
{
    public Dictionary<string, string> BoxSkins { get; set; } = new();
    public Dictionary<string, CharacterPortraitConfig> Characters { get; set; } = new();
    public Dictionary<string, SceneConfig> Scenes { get; set; } = new();

    [JsonPropertyName("_comment")]
    public string? Comment { get; set; }

    [JsonPropertyName("_scenesComment")]
    public string? ScenesComment { get; set; }
}
