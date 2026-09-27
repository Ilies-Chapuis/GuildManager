using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuildManager.Logic.Dialogues;

/// <summary>
/// Charge portraits.json, met les spritesheets en cache, et decoupe le portrait
/// (personnage + emotion) demande en un CroppedBitmap pret a afficher.
/// </summary>
public class PortraitLibrary
{
    private readonly Dictionary<string, CharacterPortraitConfig> _characters;
    private readonly Dictionary<string, string> _boxSkins;
    private readonly Dictionary<string, SceneConfig> _scenes;
    private readonly Dictionary<string, BitmapImage> _sheetCache = new();
    private readonly string _portraitsFolder;
    private readonly string _boxSkinsFolder;
    private readonly string _scenesFolder;

    /// <param name="portraitsJsonPath">Chemin vers Data/Portraits/portraits.json.</param>
    /// <param name="portraitsFolder">Dossier contenant les PNG de spritesheets (ex: Assets/Portraits).</param>
    /// <param name="boxSkinsFolder">Dossier contenant les PNG de boites de dialogue (ex: Assets/DialogueBoxes).</param>
    /// <param name="scenesFolder">Dossier contenant les CG de boss (ex: Assets/Scenes). Par defaut = portraitsFolder.</param>
    public PortraitLibrary(string portraitsJsonPath, string portraitsFolder, string boxSkinsFolder, string? scenesFolder = null)
    {
        _portraitsFolder = portraitsFolder;
        _boxSkinsFolder = boxSkinsFolder;
        _scenesFolder = scenesFolder ?? portraitsFolder;

        var json = File.ReadAllText(portraitsJsonPath);
        var data = JsonSerializer.Deserialize<PortraitLibraryData>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PortraitLibraryData();

        _characters = data.Characters;
        _boxSkins = data.BoxSkins;
        _scenes = data.Scenes;
    }

    /// <summary>Renvoie la config de scene CG pour ce personnage (ex: "Grendel"), ou null si c'est un portrait classique.</summary>
    public SceneConfig? TryGetScene(string character) =>
        !string.IsNullOrEmpty(character) && _scenes.TryGetValue(character, out var scene) ? scene : null;

    /// <summary>Charge l'illustration de fond d'une scene CG.</summary>
    public ImageSource? GetSceneBackground(SceneConfig scene) =>
        LoadBitmap(Path.Combine(_scenesFolder, scene.Image));

    /// <summary>
    /// Portrait decoupe pour ce personnage/emotion, ou null si le personnage n'a pas
    /// de spritesheet configuree (ex: "Narrator") -> la vue doit alors masquer le portrait.
    /// </summary>
    public ImageSource? GetPortrait(string character, string? emotion)
    {
        if (string.IsNullOrEmpty(character) || !_characters.TryGetValue(character, out var config))
            return null;

        var sheet = GetOrLoadSheet(config.SpriteSheet);
        if (sheet == null) return null;

        int index = ResolveEmotionIndex(config, emotion);

        double cellW = sheet.PixelWidth / (double)config.Columns;
        double cellH = sheet.PixelHeight / (double)config.Rows;

        int col = index % config.Columns;
        int row = index / config.Columns;

        int x = (int)Math.Round(col * cellW);
        int y = (int)Math.Round(row * cellH);
        int w = (int)Math.Round(cellW);
        int h = (int)Math.Round(cellH);

        // Garde-fou : evite qu'un arrondi ne sorte du bitmap sur le dernier bord.
        x = Math.Max(0, Math.Min(x, sheet.PixelWidth - 1));
        y = Math.Max(0, Math.Min(y, sheet.PixelHeight - 1));
        w = Math.Max(1, Math.Min(w, sheet.PixelWidth - x));
        h = Math.Max(1, Math.Min(h, sheet.PixelHeight - y));

        return new CroppedBitmap(sheet, new Int32Rect(x, y, w, h));
    }

    /// <summary>Image de fond de boite pour la cle donnee ("Nature", etc.), ou le skin "Default" a defaut.</summary>
    public ImageSource? GetBoxSkin(string? skinKey)
    {
        var key = string.IsNullOrEmpty(skinKey) ? "Default" : skinKey;

        if (!_boxSkins.TryGetValue(key, out var file) && !_boxSkins.TryGetValue("Default", out file))
            return null;

        return LoadBitmap(Path.Combine(_boxSkinsFolder, file!));
    }

    private int ResolveEmotionIndex(CharacterPortraitConfig config, string? emotion)
    {
        if (string.IsNullOrEmpty(emotion))
            return config.Emotions.TryGetValue("Neutral", out var neutral) ? neutral : 0;

        if (config.Emotions.TryGetValue(emotion, out var idx))
            return idx;

        // Permet de tester avec un index brut ("7") sans avoir a nommer l'emotion.
        if (int.TryParse(emotion, out var raw))
            return raw;

        return config.Emotions.TryGetValue("Neutral", out var neutral2) ? neutral2 : 0;
    }

    private BitmapImage? GetOrLoadSheet(string fileName)
    {
        if (_sheetCache.TryGetValue(fileName, out var cached))
            return cached;

        var bmp = LoadBitmap(Path.Combine(_portraitsFolder, fileName));
        if (bmp != null) _sheetCache[fileName] = bmp;
        return bmp;
    }

    private static BitmapImage? LoadBitmap(string path)
    {
        if (!File.Exists(path)) return null;

        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad; // charge en pleine resolution, fichier liberable ensuite
        bmp.UriSource = new Uri(path, UriKind.Absolute);
        bmp.EndInit();
        bmp.Freeze(); // thread-safe + partageable dans le cache
        return bmp;
    }
}
