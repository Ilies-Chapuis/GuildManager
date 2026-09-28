using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using GuildManager.Logic.Dialogues;

namespace GuildManager;

public partial class App : Application
{
    private static PortraitLibrary? _portraits;
    private static DialogueService? _dialogues;

    // Shared portrait/dialogue readers, lazily built once from the files
    // copied next to the .exe (Assets/Portraits, Assets/DialogueBoxes,
    // Assets/Scenes, Logic/Data/dialogues.json). Any view can use these
    // without re-parsing the JSON every time.
    public static PortraitLibrary Portraits => _portraits ??= new PortraitLibrary(
        portraitsJsonPath: Path.Combine(AppContext.BaseDirectory, "Assets", "Portraits", "portraits.json"),
        portraitsFolder: Path.Combine(AppContext.BaseDirectory, "Assets", "Portraits"),
        boxSkinsFolder: Path.Combine(AppContext.BaseDirectory, "Assets", "DialogueBoxes"),
        scenesFolder: Path.Combine(AppContext.BaseDirectory, "Assets", "Scenes"));

    public static DialogueService Dialogues => _dialogues ??= new DialogueService(
        Path.Combine(AppContext.BaseDirectory, "Data", "dialogues.json"));

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Catches any exception thrown on the UI thread and shows it instead
        // of letting the app die silently.
        DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show(
                args.Exception.ToString(),
                "Erreur non gérée",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            args.Handled = true; // prevents an immediate crash, gives time to read
        };

        // Catches exceptions from any background thread too.
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            MessageBox.Show(
                (args.ExceptionObject as Exception)?.ToString() ?? "Erreur inconnue",
                "Erreur fatale",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        };
    }
}
