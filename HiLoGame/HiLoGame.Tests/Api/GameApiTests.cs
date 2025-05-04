// Projeto: HiLoGame.Tests (API Integration)
// Requer pacotes: Microsoft.AspNetCore.Mvc.Testing, xUnit, System.Net.Http.Json

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace HiLoGame.Tests.Api;

public class GameApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly string[] collection = new[] { "HI", "LO", "CORRECT" };

    public GameApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_And_Join_Game_And_Guess()
    {
        var createResponse = await _client.PostAsJsonAsync("/games", new { Min = 1, Max = 10 });
        createResponse.EnsureSuccessStatusCode();
        var game = await createResponse.Content.ReadFromJsonAsync<CreateGameResponse>();

        Assert.NotNull(game);
        Assert.True(game!.Id != Guid.Empty);

        var joinResponse = await _client.PostAsJsonAsync($"/games/{game.Id}/join", new { PlayerName = "Tester" });
        joinResponse.EnsureSuccessStatusCode();
        var player = await joinResponse.Content.ReadFromJsonAsync<JoinGameResponse>();

        Assert.NotNull(player);
        Assert.True(player!.Id != Guid.Empty);

        var guessResponse =
            await _client.PostAsJsonAsync($"/games/{game.Id}/guess", new { PlayerId = player.Id, Guess = 5 });
        guessResponse.EnsureSuccessStatusCode();
        var guessResult = await guessResponse.Content.ReadFromJsonAsync<GuessResultResponse>();

        Assert.NotNull(guessResult);
        Assert.Contains(guessResult!.Result, collection);
    }

    [Fact]
    public async Task Should_Throttle_Requests_When_Exceeding_Limit()
    {
        var createResponse = await _client.PostAsJsonAsync("/games", new { min = 1, max = 100 });
        createResponse.EnsureSuccessStatusCode();
        var game = await createResponse.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(game);

        var joinResponse = await _client.PostAsJsonAsync($"/games/{game!.Id}/join", new { playerName = "Tester" });
        joinResponse.EnsureSuccessStatusCode();
        var player = await joinResponse.Content.ReadFromJsonAsync<JoinGameResponse>();
        Assert.NotNull(player);

        var tasks = new List<Task<HttpResponseMessage>>();

        for (var i = 0; i < 7; i++)
        {
            tasks.Add(_client.PostAsJsonAsync($"/games/{game.Id}/guess", new { playerId = player.Id, guess = 100 }));
        }

        var responses = await Task.WhenAll(tasks);

        int successCount = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
        int throttledCount = responses.Count(r => r.StatusCode == (HttpStatusCode)429);

        Assert.Equal(5, successCount);
        Assert.Equal(2, throttledCount);
    }

    private record CreateGameResponse(Guid Id, int Min, int Max);

    private record JoinGameResponse(Guid Id, string Name);

    private record GuessResultResponse(string Result);
}