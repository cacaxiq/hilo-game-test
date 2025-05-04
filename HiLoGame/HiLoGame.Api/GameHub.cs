namespace HiLoGame.Api;

using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public async Task SendGuessResult(Guid gameId, string result)
    {
        await Clients.Group(gameId.ToString()).SendAsync("ReceiveGuessResult", result);
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext?.Request.Query.TryGetValue("gameId", out var gameId) == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        await base.OnConnectedAsync();
    }
}
