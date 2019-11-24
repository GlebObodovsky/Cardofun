using System;
using Cardofun.Core.Enums;

namespace Cardofun.Core.ApiParameters
{
    public class UserFriendParams: UserParams
    {
        /// <summary>
        /// Friendship status is Accepted by default
        /// </summary>
        /// <value></value>
        public FriendshipStatus[] FriendshipStatus { get; set; } = new FriendshipStatus[] { Enums.FriendshipStatus.Accepted };
        /// <summary>
        /// Null - All friendships (default)
        /// True - Friendships owned by the requested user
        /// False - Friendships owned by users that proposed the requested user  
        /// </summary>
        /// <value></value>
        public Boolean? IsFriendshipOwned { get; set; }
    }
}