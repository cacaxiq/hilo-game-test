using HiLoGame.Application.Interfaces;
using HiLoGame.Domain.Entities;
using System.Collections.Concurrent;

namespace HiLoGame.Infrastructure.Repositories;

public class InMemoryGameRepository : IGameRepository
{
    private static readonly ConcurrentDictionary<Guid, Game> _games = new();

    public Task<Game?> GetByIdAsync(Guid id)
    {
        _games.TryGetValue(id, out var game);
        return Task.FromResult(game);
    }

    public Task SaveAsync(Game game)
    {
        _games[game.Id] = game;
        return Task.CompletedTask;
    }
}