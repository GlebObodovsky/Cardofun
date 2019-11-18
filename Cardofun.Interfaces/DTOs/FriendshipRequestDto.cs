using System;
using Cardofun.Core.Enums;

namespace Cardofun.Interfaces.DTOs
{
    public class FriendshipRequestDto
    {
        /// <summary>
        /// Determines if the user is the one who firstly initiated the friendship
        /// </summary>
        /// <value></value>
        public Boolean IsOwner { get; set; }

        /// <summary>
        /// Status of the friendship
        /// </summary>
        /// <value></value>
        public FriendshipStatus Status { get; set; }
    }
}