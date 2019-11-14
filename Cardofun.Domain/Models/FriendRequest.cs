using System;
using Cardofun.Core.Enums;

namespace Cardofun.Domain.Models
{
    /// <summary>
    /// Represents a user friendship request
    /// </summary>
    public class FriendRequest
    {
        /// <summary>
        /// Id of a user that requested the friendship
        /// </summary>
        /// <value></value>
        public Int32 FromUserId { get; set; }
        public virtual User FromUser { get; set; }
        /// <summary>
        /// Id of a user that was requested for the friendship
        /// </summary>
        /// <value></value>
        public Int32 ToUserId { get; set; }
        public virtual User ToUser { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RepliedAt { get; set; }
    }
}