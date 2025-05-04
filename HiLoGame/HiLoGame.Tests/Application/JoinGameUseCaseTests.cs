using HiLoGame.Application.Interfaces;
using HiLoGame.Application.UseCases;
using HiLoGame.Domain.Entities;
using Moq;

namespace HiLoGame.Tests.Application;

public class JoinGameUseCaseTests
{
    [Fact]
    public async Task Should_Add_Player_To_Game()
    {
        var game = new Game(1, 10);
        var repoMock = new Mock<IGameRepository>();
        repoMock.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game);
        var useCase = new JoinGameUseCase(repoMock.Object);

        var player = await useCase.ExecuteAsync(game.Id, "Alice");

        Assert.Contains(game.Players, p => p.Id == player.Id);
        repoMock.Verify(r => r.SaveAsync(game), Times.Once);
    }
}