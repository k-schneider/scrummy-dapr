namespace Scrummy.GameService.Api.Hubs;

public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (string.IsNullOrWhiteSpace(Context.UserIdentifier))
        {
            throw new Exception("Request does not contain a sid");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        await base.OnDisconnectedAsync(ex);
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }
}
