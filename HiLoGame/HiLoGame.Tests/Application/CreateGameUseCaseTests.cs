using HiLoGame.Application.UseCases;
using HiLoGame.Domain.Entities;
using HiLoGame.Application.Interfaces;
using Moq;
using Xunit;

namespace HiLoGame.Tests.Application;

public class CreateGameUseCaseTests
{
    [Fact]
    public async Task Should_Create_And_Save_Game()
    {
        var repoMock = new Mock<IGameRepository>();
        var useCase = new CreateGameUseCase(repoMock.Object);

        var game = await useCase.ExecuteAsync(1, 10);

        Assert.InRange(game.MysteryNumber, 1, 10);
        repoMock.Verify(r => r.SaveAsync(It.IsAny<Game>()), Times.Once);
    }
}