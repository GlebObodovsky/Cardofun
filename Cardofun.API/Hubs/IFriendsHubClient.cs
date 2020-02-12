using System;
using System.Threading.Tasks;

namespace Cardofun.API.Hubs
{
    public interface IFriendsHubClient
    {
        Task ReceiveFollowersCount(Int32 count);
    }
}