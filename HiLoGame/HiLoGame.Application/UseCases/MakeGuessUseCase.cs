using HiLoGame.Application.Interfaces;
using HiLoGame.Domain.Entities;

namespace HiLoGame.Application.UseCases;

public class MakeGuessUseCase(IGameRepository repository)
{
    public async Task<GuessResult> ExecuteAsync(Guid gameId, Guid playerId, int guess)
    {
        var game = await repository.GetByIdAsync(gameId)
                   ?? throw new ArgumentException("Game not found");

        var result = game.MakeGuess(playerId, guess);
        await repository.SaveAsync(game);
        return result;
    }
}