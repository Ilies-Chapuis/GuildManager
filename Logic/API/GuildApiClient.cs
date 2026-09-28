using System.Net.Http.Json;
using GuildManager.Logic.API.Dtos;

namespace GuildManager.Logic.API;

// Talks to the /guilds/* endpoints of GuildManager.Api: creating/joining a
// guild, recruiting adventurers, and advancing the day. Matches the
// server's actual routes exactly - see Logic/API/GuildManager.Api/Program.cs
// on the API project. Not wired into any UI screen yet: this class only
// provides the client-side plumbing for the online mode.
public sealed class GuildApiClient
{
    private readonly HttpClient _http;

    public GuildApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public GuildApiClient(HttpClient httpClient)
    {
        _http = httpClient;
    }

    // POST /guilds - creates a new online guild, owned by the given player.
    public async Task<GuildStateDto> CreateGuildAsync(int playerId, string name)
    {
        var request = new CreateGuildRequestDto { PlayerId = playerId, Name = name };
        var response = await _http.PostAsJsonAsync("/guilds", request);
        return await ReadOrThrowAsync<GuildStateDto>(response);
    }

    // POST /guilds/{guildId}/join - adds a second player to an existing guild.
    public async Task<GuildStateDto> JoinGuildAsync(int guildId, int playerId)
    {
        var request = new JoinGuildRequestDto { PlayerId = playerId };
        var response = await _http.PostAsJsonAsync($"/guilds/{guildId}/join", request);
        return await ReadOrThrowAsync<GuildStateDto>(response);
    }

    // GET /guilds/{guildId} - fetches the guild's current shared state.
    public async Task<GuildStateDto> GetGuildAsync(int guildId)
    {
        var response = await _http.GetAsync($"/guilds/{guildId}");
        return await ReadOrThrowAsync<GuildStateDto>(response);
    }

    // POST /guilds/{guildId}/adventurers - recruits a new adventurer, owned
    // by the given player. category is "Warrior"/"Healer"/"Mage"/"Special";
    // specialKey (e.g. "Sameth") is required only for "Special".
    public async Task<AdventurerStateDto> RecruitAdventurerAsync(int guildId, int playerId, string name, string category, string? specialKey = null)
    {
        var request = new RecruitAdventurerRequestDto
        {
            PlayerId = playerId,
            Name = name,
            Category = category,
            SpecialKey = specialKey
        };
        var response = await _http.PostAsJsonAsync($"/guilds/{guildId}/adventurers", request);
        return await ReadOrThrowAsync<AdventurerStateDto>(response);
    }

    // GET /guilds/{guildId}/adventurers - lists every adventurer in the
    // guild, optionally filtered to one player's own recruits.
    public async Task<List<AdventurerStateDto>> ListAdventurersAsync(int guildId, int? playerId = null)
    {
        string url = playerId is null
            ? $"/guilds/{guildId}/adventurers"
            : $"/guilds/{guildId}/adventurers?playerId={playerId}";

        var response = await _http.GetAsync(url);
        return await ReadOrThrowAsync<List<AdventurerStateDto>>(response);
    }

    // POST /guilds/{guildId}/advance-day - moves the shared guild to the
    // next day and resets every adventurer's "used today" flag.
    public async Task<DayAdvanceResultDto> AdvanceDayAsync(int guildId)
    {
        var response = await _http.PostAsync($"/guilds/{guildId}/advance-day", content: null);
        return await ReadOrThrowAsync<DayAdvanceResultDto>(response);
    }

    // POST /guilds/{guildId}/evaluate-ending - checks the day-10 win
    // condition (Gold >= 5000) and records the guild's ending.
    public async Task<EndingResultDto> EvaluateEndingAsync(int guildId)
    {
        var response = await _http.PostAsync($"/guilds/{guildId}/evaluate-ending", content: null);
        return await ReadOrThrowAsync<EndingResultDto>(response);
    }

    private static async Task<T> ReadOrThrowAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync();
            throw new ApiException(response.StatusCode, string.IsNullOrWhiteSpace(body) ? response.ReasonPhrase ?? "Request failed." : body);
        }

        return await response.Content.ReadFromJsonAsync<T>()
               ?? throw new ApiException(response.StatusCode, "The server returned an empty response.");
    }
}
