using HiLoGame.Application.UseCases;
using HiLoGame.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace HiLoGame.Api;

public class GuessHandler
{
    private readonly MakeGuessUseCase _useCase;
    private readonly IHubContext<GameHub> _hubContext;

    public GuessHandler(MakeGuessUseCase useCase, IHubContext<GameHub> hubContext)
    {
        _useCase = useCase;
        _hubContext = hubContext;
    }

    public async Task<GuessResult> ExecuteAsync(Guid gameId, Guid playerId, int guess)
    {
        var result = await _useCase.ExecuteAsync(gameId, playerId, guess);

        await _hubContext.Clients.Group(gameId.ToString())
            .SendAsync("ReceiveGuessResult", new
            {
                PlayerId = playerId,
                Result = result.ToString()
            });

        return result;
    }
}
