using HiLoGame.Application.Interfaces;
using HiLoGame.Domain.Entities;

namespace HiLoGame.Application.UseCases;

public class JoinGameUseCase(IGameRepository repository)
{
    public async Task<Player> ExecuteAsync(Guid gameId, string playerName)
    {
        var game = await repository.GetByIdAsync(gameId)
                   ?? throw new ArgumentException("Game not found");

        var player = new Player { Name = playerName };
        game.JoinPlayer(player);
        await repository.SaveAsync(game);
        return player;
    }
}