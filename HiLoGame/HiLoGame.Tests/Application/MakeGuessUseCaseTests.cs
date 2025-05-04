using HiLoGame.Application.Interfaces;
using HiLoGame.Application.UseCases;
using HiLoGame.Domain.Entities;
using Moq;

namespace HiLoGame.Tests.Application;

public class MakeGuessUseCaseTests
{
    [Fact]
    public async Task Should_Return_Correct_When_Guess_Is_Exact()
    {
        var game = new Game(1, 10);
        var player = new Player { Name = "Bob" };
        game.JoinPlayer(player);
        typeof(Game).GetProperty("MysteryNumber")!.SetValue(game, 7);

        var repoMock = new Mock<IGameRepository>();
        repoMock.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game);
        var useCase = new MakeGuessUseCase(repoMock.Object);

        var result = await useCase.ExecuteAsync(game.Id, player.Id, 7);

        Assert.Equal(GuessResult.CORRECT, result);
        repoMock.Verify(r => r.SaveAsync(game), Times.Once);
    }
}