using HiLoGame.Application.Interfaces;
using HiLoGame.Domain.Entities;

namespace HiLoGame.Application.UseCases;

public class CreateGameUseCase(IGameRepository repository)
{
    public async Task<Game> ExecuteAsync(int min, int max)
    {
        var game = new Game(min, max);
        await repository.SaveAsync(game);
        return game;
    }
}