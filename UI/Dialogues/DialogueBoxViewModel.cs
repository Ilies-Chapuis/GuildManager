using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Threading;
using GuildManager.Logic.Dialogues;

namespace GuildManager.UI.Dialogues;

/// <summary>
/// Pilote une sequence de DialogueEntry : effet machine a ecrire, changement de
/// portrait/skin de boite selon la ligne courante, et bascule Serious/Wtf.
/// </summary>
public class DialogueBoxViewModel : INotifyPropertyChanged
{
    private readonly PortraitLibrary _portraits;
    private readonly DispatcherTimer _typeTimer;
    private Queue<DialogueEntry> _queue = new();
    private string _fullText = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Declenche quand la file de dialogue est entierement jouee et que la boite doit se fermer.</summary>
    public event Action? Finished;

    public DialogueBoxViewModel(PortraitLibrary portraits, TimeSpan? typeSpeed = null)
    {
        _portraits = portraits;
        _typeTimer = new DispatcherTimer { Interval = typeSpeed ?? TimeSpan.FromMilliseconds(22) };
        _typeTimer.Tick += OnTypeTick;
    }

    // ---- Mode Serieux / WTF ----
    private bool _isWtfMode;
    public bool IsWtfMode
    {
        get => _isWtfMode;
        set
        {
            if (Set(ref _isWtfMode, value) && Current != null)
                StartTyping(Current.GetText(_isWtfMode));
        }
    }

    // ---- Etat courant ----
    private DialogueEntry? _current;
    public DialogueEntry? Current
    {
        get => _current;
        private set { Set(ref _current, value); OnPropertyChanged(nameof(IsVisible)); OnPropertyChanged(nameof(CharacterDisplayName)); }
    }

    public bool IsVisible => Current != null;
    public string CharacterDisplayName => Current?.Character ?? "";
    public bool HasMoreQueued => _queue.Count > 0;

    private string _displayedText = "";
    public string DisplayedText { get => _displayedText; private set => Set(ref _displayedText, value); }

    private bool _isTyping;
    public bool IsTyping { get => _isTyping; private set => Set(ref _isTyping, value); }

    private ImageSource? _portraitSource;
    public ImageSource? PortraitSource { get => _portraitSource; private set => Set(ref _portraitSource, value); }

    private ImageSource? _boxSkinSource;
    public ImageSource? BoxSkinSource { get => _boxSkinSource; private set => Set(ref _boxSkinSource, value); }

    // ---- Mode "scene CG" (boss plein ecran, ex: Grendel / Nyxaria) ----
    private bool _isSceneMode;
    public bool IsSceneMode
    {
        get => _isSceneMode;
        private set { if (Set(ref _isSceneMode, value)) OnPropertyChanged(nameof(IsBottomBarMode)); }
    }

    /// <summary>Inverse pratique de IsSceneMode pour le binding XAML (Visibility de la barre du bas).</summary>
    public bool IsBottomBarMode => !IsSceneMode;

    private ImageSource? _sceneBackground;
    public ImageSource? SceneBackground { get => _sceneBackground; private set => Set(ref _sceneBackground, value); }

    private double _sceneCanvasWidth;
    public double SceneCanvasWidth { get => _sceneCanvasWidth; private set => Set(ref _sceneCanvasWidth, value); }

    private double _sceneCanvasHeight;
    public double SceneCanvasHeight { get => _sceneCanvasHeight; private set => Set(ref _sceneCanvasHeight, value); }

    private double _sceneTextX, _sceneTextY, _sceneTextWidth, _sceneTextHeight;
    public double SceneTextX { get => _sceneTextX; private set => Set(ref _sceneTextX, value); }
    public double SceneTextY { get => _sceneTextY; private set => Set(ref _sceneTextY, value); }
    public double SceneTextWidth { get => _sceneTextWidth; private set => Set(ref _sceneTextWidth, value); }
    public double SceneTextHeight { get => _sceneTextHeight; private set => Set(ref _sceneTextHeight, value); }

    /// <summary>true si la scene doit dessiner son propre panneau (le CG n'a pas de boite integree).</summary>
    private bool _sceneNeedsOwnBox;
    public bool SceneNeedsOwnBox { get => _sceneNeedsOwnBox; private set => Set(ref _sceneNeedsOwnBox, value); }

    // ---- API publique ----

    /// <summary>Lance une sequence de plusieurs lignes (ex: tout le Day N).</summary>
    public void Play(IEnumerable<DialogueEntry> entries)
    {
        _queue = new Queue<DialogueEntry>(entries);
        ShowNext();
    }

    /// <summary>Lance une seule ligne (ex: une reaction ponctuelle).</summary>
    public void PlaySingle(DialogueEntry entry)
    {
        _queue = new Queue<DialogueEntry>();
        _queue.Enqueue(entry);
        ShowNext();
    }

    /// <summary>
    /// A appeler sur clic / touche de confirmation. Termine instantanement le texte
    /// en cours de frappe, ou passe a la ligne suivante si le texte est deja complet.
    /// </summary>
    public void Advance()
    {
        if (IsTyping)
        {
            CompleteTyping();
            return;
        }
        ShowNext();
    }

    // ---- Interne ----

    private void ShowNext()
    {
        if (_queue.Count == 0)
        {
            Current = null;
            Finished?.Invoke();
            return;
        }

        Current = _queue.Dequeue();
        OnPropertyChanged(nameof(HasMoreQueued));

        var scene = _portraits.TryGetScene(Current.Character);
        if (scene != null)
        {
            IsSceneMode = true;
            SceneBackground = _portraits.GetSceneBackground(scene);
            SceneCanvasWidth = scene.CanvasWidth;
            SceneCanvasHeight = scene.CanvasHeight;
            SceneTextX = scene.TextRect.X;
            SceneTextY = scene.TextRect.Y;
            SceneTextWidth = scene.TextRect.Width;
            SceneTextHeight = scene.TextRect.Height;
            SceneNeedsOwnBox = !scene.HasBakedBox;

            PortraitSource = null;
            BoxSkinSource = null;
        }
        else
        {
            IsSceneMode = false;
            PortraitSource = _portraits.GetPortrait(Current.Character, Current.Emotion);
            BoxSkinSource = _portraits.GetBoxSkin(Current.BoxSkin);
        }

        StartTyping(Current.GetText(IsWtfMode));
    }

    private void StartTyping(string text)
    {
        _fullText = text ?? "";
        DisplayedText = "";
        _typeTimer.Stop();

        IsTyping = _fullText.Length > 0;
        if (IsTyping) _typeTimer.Start();
    }

    private void OnTypeTick(object? sender, EventArgs e)
    {
        if (DisplayedText.Length >= _fullText.Length)
        {
            CompleteTyping();
            return;
        }
        DisplayedText += _fullText[DisplayedText.Length];
    }

    private void CompleteTyping()
    {
        _typeTimer.Stop();
        DisplayedText = _fullText;
        IsTyping = false;
    }

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }

    private void OnPropertyChanged(string? name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
