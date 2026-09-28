using System.Net.Http.Json;
using GuildManager.Logic.API.Dtos;

namespace GuildManager.Logic.API;

// Thrown when the API returns an error status. Message carries the server's
// response body (e.g. "Username or email already taken.") when available.
public sealed class ApiException : Exception
{
    public System.Net.HttpStatusCode StatusCode { get; }

    public ApiException(System.Net.HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}

// Talks to the /auth/* endpoints of GuildManager.Api (register/login).
// Matches the server's actual routes exactly - see Logic/API/GuildManager.Api/Program.cs
// on the API project. Not wired into any UI screen yet: this class only
// provides the client-side plumbing for whoever builds the online login
// screen.
public sealed class AuthApiClient
{
    private readonly HttpClient _http;

    // baseUrl should point at the running GuildManager.Api instance (e.g.
    // "https://localhost:7000" or whatever port it listens on locally).
    public AuthApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    // Allows reusing an already-configured HttpClient (e.g. shared across
    // AuthApiClient/GuildApiClient/QuestApiClient) instead of creating a new one.
    public AuthApiClient(HttpClient httpClient)
    {
        _http = httpClient;
    }

    // POST /auth/register - creates a new player account.
    public async Task<PlayerDto> RegisterAsync(string username, string email, string password)
    {
        var request = new RegisterRequestDto { Username = username, Email = email, Password = password };
        var response = await _http.PostAsJsonAsync("/auth/register", request);
        return await ReadOrThrowAsync<PlayerDto>(response);
    }

    // POST /auth/login - returns the player's info if the credentials are valid.
    public async Task<PlayerDto> LoginAsync(string username, string password)
    {
        var request = new LoginRequestDto { Username = username, Password = password };
        var response = await _http.PostAsJsonAsync("/auth/login", request);
        return await ReadOrThrowAsync<PlayerDto>(response);
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
