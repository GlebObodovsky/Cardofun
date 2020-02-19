using System;
using System.Threading.Tasks;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.API.Hubs
{
    public interface IFriendsHubClient
    {
        Task ReceiveFollowersCount(Int32 count);
        Task ReceiveIncommingFriendshipRequest(UserForListDto newFollower);
        Task ReceiveOutgoingFriendshipRequest(UserForListDto newFollower);
        Task ReceiveAcceptedFriendship(UserForListDto newFollower);
        Task ReceiveFriendshipStatus(FriendshipRequestStatusDto friendshipState);
    }
}