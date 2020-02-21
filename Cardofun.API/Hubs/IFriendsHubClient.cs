using System.Threading.Tasks;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.API.Hubs
{
    public interface IFriendsHubClient
    {
        Task ReceiveIncommingFriendshipRequest(UserForListDto newFollower);
        Task ReceiveOutgoingFriendshipRequest(UserForListDto newFollower);
        Task ReceiveAcceptedFriendship(UserForListDto newFollower);
        Task ReceiveFriendshipStatus(FriendshipRequestStatusDto friendshipState);
    }
}