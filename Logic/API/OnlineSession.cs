using GuildManager.Logic.API.Dtos;

namespace GuildManager.Logic.API;

// Online-mode state shared by every screen: which API server to talk to,
// the single HttpClient behind the three API clients, and who is currently
// logged in. The solo mode never touches this class.
public static class OnlineSession
{
    // Matches Logic/API/GuildManager.Api/Properties/launchSettings.json.
    public const string DefaultBaseUrl = "http://localhost:5211";

    private static string _baseUrl =
        Environment.GetEnvironmentVariable("GUILDMANAGER_API_URL") ?? DefaultBaseUrl;
    private static HttpClient? _http;

    // Address of the running GuildManager.Api server. Can also be set with
    // the GUILDMANAGER_API_URL environment variable. Changing it drops the
    // current connection so the next call uses the new address.
    public static string BaseUrl
    {
        get => _baseUrl;
        set
        {
            _baseUrl = value;
            _http?.Dispose();
            _http = null;
        }
    }

    public static PlayerDto? CurrentPlayer { get; private set; }

    public static bool IsLoggedIn => CurrentPlayer is not null;

    // Short timeout so the UI reports an unreachable server quickly instead
    // of appearing frozen.
    private static HttpClient Http =>
        _http ??= new HttpClient { BaseAddress = new Uri(_baseUrl), Timeout = TimeSpan.FromSeconds(10) };

    public static AuthApiClient Auth => new(Http);
    public static GuildApiClient Guilds => new(Http);
    public static QuestApiClient Quests => new(Http);

    // The API requires an email (unique in the database), but the login
    // screen only asks for a pseudo and a password: derive a unique
    // placeholder address from the pseudo.
    public static string BuildPlaceholderEmail(string username) =>
        $"{username.Trim().ToLowerInvariant()}@guildmanager.local";

    // Creates the account, then logs in with it.
    public static async Task<PlayerDto> RegisterAsync(string username, string password)
    {
        username = username.Trim();
        await Auth.RegisterAsync(username, BuildPlaceholderEmail(username), password);
        return await LoginAsync(username, password);
    }

    public static async Task<PlayerDto> LoginAsync(string username, string password)
    {
        CurrentPlayer = await Auth.LoginAsync(username.Trim(), password);
        return CurrentPlayer;
    }

    public static void Logout() => CurrentPlayer = null;
}
