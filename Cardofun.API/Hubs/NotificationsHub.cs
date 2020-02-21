using Microsoft.AspNetCore.SignalR;

namespace Cardofun.API.Hubs
{
    public class NotificationsHub: Hub<INotificationsHubClient> {}
}