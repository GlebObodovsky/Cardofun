using Microsoft.AspNetCore.SignalR;

namespace Cardofun.API.Hubs
{
    public class ChatHub: Hub<IChatHubClient> {}
}