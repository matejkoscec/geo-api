using Microsoft.AspNetCore.SignalR;

namespace geo_api.Features.Location;

public interface ILocationClient
{
    Task ReceiveMessage(string message);
}

public class NearbySearchHub : Hub<ILocationClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Client(Context.ConnectionId)
            .ReceiveMessage($"Connected to {nameof(NearbySearchHub)} with id '{Context.ConnectionId}'.");
    }
}