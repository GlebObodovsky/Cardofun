using System;
using System.Collections.Generic;

namespace Cardofun.Interfaces.DTOs
{
    public class ReadMessagesListDto
    {
        /// <summary>
        /// Id of a user that is participating in chat
        /// </summary>
        /// <value></value>
        public Int32 UserOneId { get; set; }
        /// <summary>
        /// Id of a user that is participating in chat
        /// </summary>
        /// <value></value>
        public Int32 UserTwoId { get; set; }
        /// <summary>
        /// Ids of the messages that are marked as read in the chat between UserOne and UserTwo
        /// </summary>
        /// <value></value>
        public IEnumerable<Guid> MessageIds { get; set; }
    }
}