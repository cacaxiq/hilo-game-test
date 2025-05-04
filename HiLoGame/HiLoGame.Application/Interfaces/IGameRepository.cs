using HiLoGame.Domain.Entities;

namespace HiLoGame.Application.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id);
    Task SaveAsync(Game game);
}