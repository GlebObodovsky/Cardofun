using System;
using System.Threading.Tasks;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.API.Hubs
{
    public interface IFriendsHubClient
    {
        Task ReceiveFollowersCount(Int32 count);
        Task ReceiveFriendshipRequest(UserForListDto newFollower);
        Task ReceiveFriendshipStatus(FriendshipRequestStatusDto friendshipState);
    }
}