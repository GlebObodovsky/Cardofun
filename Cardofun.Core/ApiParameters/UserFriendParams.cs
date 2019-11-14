using Cardofun.Core.Enums;

namespace Cardofun.Core.ApiParameters
{
    public class UserFriendParams: UserParams
    {
        /// <summary>
        /// Friendship status is Accepted by default
        /// </summary>
        /// <value></value>
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Accepted;
    }
}