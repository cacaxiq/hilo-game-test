using HiLoGame.Domain.Entities;

namespace HiLoGame.Tests.Domain;

public class GameTests
{
    [Fact]
    public void Should_Create_Game_With_Valid_Configuration()
    {
        var game = new Game(1, 100);

        Assert.InRange(game.MysteryNumber, 1, 100);
        Assert.Equal(GameStatus.WaitingForPlayers, game.Status);
    }

    [Fact]
    public void Should_Join_Player_To_Game()
    {
        var game = new Game(1, 100);
        var player = new Player { Name = "Alice" };

        game.JoinPlayer(player);

        Assert.Contains(game.Players, p => p.Id == player.Id);
    }

    [Fact]
    public void Should_Return_HI_When_Guess_Is_Less_Than_Answer()
    {
        var game = new Game(1, 100);
        var player = new Player { Name = "Tester" };
        game.JoinPlayer(player);

        typeof(Game).GetProperty("MysteryNumber")!
            .SetValue(game, 50);

        var result = game.MakeGuess(player.Id, 25);

        Assert.Equal(GuessResult.HI, result);
    }

    [Fact]
    public void Should_Return_LO_When_Guess_Is_Greater_Than_Answer()
    {
        var game = new Game(1, 100);
        var player = new Player { Name = "Tester" };
        game.JoinPlayer(player);

        typeof(Game).GetProperty("MysteryNumber")!
            .SetValue(game, 50);

        var result = game.MakeGuess(player.Id, 75);

        Assert.Equal(GuessResult.LO, result);
    }

    [Fact]
    public void Should_Return_Correct_When_Guess_Is_Equal()
    {
        var game = new Game(1, 100);
        var player = new Player { Name = "Tester" };
        game.JoinPlayer(player);

        typeof(Game).GetProperty("MysteryNumber")!
            .SetValue(game, 42);

        var result = game.MakeGuess(player.Id, 42);

        Assert.Equal(GuessResult.CORRECT, result);
        Assert.Equal(GameStatus.Finished, game.Status);
        Assert.True(player.HasWon);
    }
}