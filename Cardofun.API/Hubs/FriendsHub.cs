using Microsoft.AspNetCore.SignalR;

namespace Cardofun.API.Hubs
{
    public class FriendsHub: Hub<IFriendsHubClient> {}
}