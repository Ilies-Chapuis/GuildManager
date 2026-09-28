using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuildManager.Logic.API;
using Xunit;

namespace GuildManager.Tests;

// Tests the three HTTP clients against a stub handler: no server, no
// database, no network. They check the routes/verbs/bodies the clients send
// (which must match GuildManager.Api's Program.cs) and how they parse or
// report the server's answers.
public class ApiClientsTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _status;
        private readonly string _body;

        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestBody { get; private set; }

        public StubHandler(HttpStatusCode status, string body)
        {
            _status = status;
            _body = body;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastRequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(_status)
            {
                Content = new StringContent(_body, Encoding.UTF8, "application/json")
            };
        }
    }

    private static HttpClient ClientFor(StubHandler handler) =>
        new(handler) { BaseAddress = new Uri("http://localhost:5211") };

    [Fact]
    public async Task RegisterAsync_PostsToAuthRegister_AndParsesThePlayer()
    {
        var handler = new StubHandler(HttpStatusCode.OK, "{\"id\":7,\"username\":\"melvin\",\"email\":\"m@x.fr\"}");
        var client = new AuthApiClient(ClientFor(handler));

        var player = await client.RegisterAsync("melvin", "m@x.fr", "secret123");

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/auth/register", handler.LastRequest.RequestUri!.AbsolutePath);
        Assert.Contains("\"username\":\"melvin\"", handler.LastRequestBody!);
        Assert.Equal(7, player.Id);
        Assert.Equal("melvin", player.Username);
    }

    [Fact]
    public async Task LoginAsync_Unauthorized_ThrowsApiExceptionCarryingTheStatus()
    {
        var handler = new StubHandler(HttpStatusCode.Unauthorized, "");
        var client = new AuthApiClient(ClientFor(handler));

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.LoginAsync("melvin", "wrong"));

        Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
    }

    [Fact]
    public async Task RegisterAsync_Conflict_ExposesTheServerMessage()
    {
        var handler = new StubHandler(HttpStatusCode.Conflict, "\"Username or email already taken.\"");
        var client = new AuthApiClient(ClientFor(handler));

        var ex = await Assert.ThrowsAsync<ApiException>(() => client.RegisterAsync("melvin", "m@x.fr", "pw"));

        Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);
        Assert.Contains("already taken", ex.Message);
    }

    [Fact]
    public async Task CreateGuildAsync_PostsToGuilds_AndParsesTheGuildState()
    {
        var handler = new StubHandler(HttpStatusCode.OK,
            "{\"id\":1,\"name\":\"Tartaros\",\"currentDay\":1,\"remainingHours\":16,\"gold\":500,\"food\":20,\"healthPotions\":10,\"ending\":\"None\",\"memberUsernames\":[\"melvin\"]}");
        var client = new GuildApiClient(ClientFor(handler));

        var guild = await client.CreateGuildAsync(playerId: 1, name: "Tartaros");

        Assert.Equal("/guilds", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Equal(500, guild.Gold);
        Assert.Equal(20, guild.Food);
        Assert.Equal(new[] { "melvin" }, guild.MemberUsernames);
    }

    [Fact]
    public async Task ListAdventurersAsync_WithPlayerId_AddsItToTheQueryString()
    {
        var handler = new StubHandler(HttpStatusCode.OK, "[]");
        var client = new GuildApiClient(ClientFor(handler));

        await client.ListAdventurersAsync(guildId: 4, playerId: 3);

        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("/guilds/4/adventurers", handler.LastRequest.RequestUri!.AbsolutePath);
        Assert.Equal("?playerId=3", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task ListQuestsAsync_WithDayAndPlayer_BuildsBothQueryParameters()
    {
        var handler = new StubHandler(HttpStatusCode.OK, "[]");
        var client = new QuestApiClient(ClientFor(handler));

        await client.ListQuestsAsync(guildId: 1, day: 2, playerId: 5);

        Assert.Equal("/guilds/1/quests", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Equal("?day=2&playerId=5", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task AttemptQuestAsync_PostsTheAdventurerIds_AndParsesTheOutcome()
    {
        var handler = new StubHandler(HttpStatusCode.OK,
            "{\"wasSuccessful\":true,\"estimatedSuccessRate\":76,\"message\":\"Quest succeeded.\"}");
        var client = new QuestApiClient(ClientFor(handler));

        var result = await client.AttemptQuestAsync(questId: 9, adventurerIds: new[] { 1, 2 });

        Assert.Equal("/quests/9/attempt", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Equal("{\"adventurerIds\":[1,2]}", handler.LastRequestBody);
        Assert.True(result.WasSuccessful);
        Assert.Equal(76, result.EstimatedSuccessRate);
    }

    [Fact]
    public void BuildPlaceholderEmail_LowercasesAndTrimsThePseudo()
    {
        Assert.Equal("melvin@guildmanager.local", OnlineSession.BuildPlaceholderEmail("  Melvin "));
    }
}

public class OnlineErrorsTests
{
    [Fact]
    public void Describe_Conflict_MentionsTheTakenPseudo()
    {
        var ex = new ApiException(System.Net.HttpStatusCode.Conflict, "Username or email already taken.");

        Assert.Contains("déjà utilisé", OnlineErrors.Describe(ex));
    }

    [Fact]
    public void Describe_Unauthorized_SaysCredentialsAreWrong()
    {
        var ex = new ApiException(System.Net.HttpStatusCode.Unauthorized, "Unauthorized");

        Assert.Contains("incorrect", OnlineErrors.Describe(ex));
    }

    [Fact]
    public void Describe_ServerUnreachable_MentionsTheAddressBeingContacted()
    {
        string message = OnlineErrors.Describe(new System.Net.Http.HttpRequestException("connection refused"));

        Assert.Contains(OnlineSession.BaseUrl, message);
    }

    [Fact]
    public void Describe_OtherApiError_KeepsTheServerMessage()
    {
        var ex = new ApiException(System.Net.HttpStatusCode.BadRequest, "Team size must be between 1 and 5.");

        Assert.Contains("Team size must be between 1 and 5.", OnlineErrors.Describe(ex));
    }
}
