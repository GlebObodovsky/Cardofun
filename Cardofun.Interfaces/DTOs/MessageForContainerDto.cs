using System;

namespace Cardofun.Interfaces.DTOs
{
    /// <summary>
    /// Represents a last message in unser container
    /// </summary>
    public class MessageForContainerDto: MessageForReturnDto
    {
        public String SenderName { get; set; }
        public String RecipientName { get; set; }
        public String SenderPhotoUrl { get; set; }
        public String RecipientPhotoUrl { get; set; }
    }
}