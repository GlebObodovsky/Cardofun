using System;
using Cardofun.Core.Enums;

namespace Cardofun.Interfaces.DTOs
{
    public class FriendshipRequestStatusDto
    {
        /// <summary>
        /// Id of a user that is initiated the friendship
        /// </summary>
        /// <value></value>
        public Int32 FromUserId { get; set; }
        /// <summary>
        /// Id of a user that is receiving the friendship request
        /// </summary>
        /// <value></value>
        public Int32 ToUserId { get; set; }
        /// <summary>
        /// Status of the friendship
        /// </summary>
        /// <value></value>
        public FriendshipStatus Status { get; set; }
        /// <summary>
        /// True - if the friend request was erased completely
        /// </summary>
        /// <value></value>
        public Boolean IsDeleted { get; set; }
    }
}