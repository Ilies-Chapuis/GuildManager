using System.Net.Http.Json;
using GuildManager.Logic.API.Dtos;

namespace GuildManager.Logic.API;

// Talks to the quest-related endpoints of GuildManager.Api: generating the
// day's quests, listing them, and attempting one with a team. Matches the
// server's actual routes exactly - see Logic/API/GuildManager.Api/Program.cs
// on the API project. Not wired into any UI screen yet: this class only
// provides the client-side plumbing for the online mode.
public sealed class QuestApiClient
{
    private readonly HttpClient _http;

    public QuestApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public QuestApiClient(HttpClient httpClient)
    {
        _http = httpClient;
    }

    // POST /guilds/{guildId}/quests/generate - generates and persists a new
    // batch of quests for the guild's current day.
    public async Task<List<QuestStateDto>> GenerateDailyQuestsAsync(int guildId)
    {
        var response = await _http.PostAsync($"/guilds/{guildId}/quests/generate", content: null);
        return await ReadOrThrowAsync<List<QuestStateDto>>(response);
    }

    // GET /guilds/{guildId}/quests - lists the guild's quests, optionally
    // filtered by day and/or to one player's own quests (plus shared ones).
    public async Task<List<QuestStateDto>> ListQuestsAsync(int guildId, int? day = null, int? playerId = null)
    {
        var query = new List<string>();
        if (day is not null) query.Add($"day={day}");
        if (playerId is not null) query.Add($"playerId={playerId}");
        string url = $"/guilds/{guildId}/quests" + (query.Count > 0 ? "?" + string.Join("&", query) : "");

        var response = await _http.GetAsync(url);
        return await ReadOrThrowAsync<List<QuestStateDto>>(response);
    }

    // POST /quests/{questId}/attempt - sends a team (by adventurer id) on
    // the quest and returns the outcome.
    public async Task<QuestAttemptResultDto> AttemptQuestAsync(int questId, IEnumerable<int> adventurerIds)
    {
        var request = new AttemptQuestRequestDto { AdventurerIds = adventurerIds.ToList() };
        var response = await _http.PostAsJsonAsync($"/quests/{questId}/attempt", request);
        return await ReadOrThrowAsync<QuestAttemptResultDto>(response);
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
