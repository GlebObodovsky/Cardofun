using System;
using System.Threading.Tasks;

namespace Cardofun.API.Hubs
{
    public interface INotificationsHubClient
    {
        Task ReceiveUnreadMessagesCount(Int32 count);
        Task ReceiveFollowersCount(Int32 count);
    }
}