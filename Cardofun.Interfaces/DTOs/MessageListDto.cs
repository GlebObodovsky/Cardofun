using System.Collections.Generic;

namespace Cardofun.Interfaces.DTOs
{
    public class MessageListDto
    {
        public IEnumerable<UserForMessageListDto> Users { get; set; }
        public IEnumerable<MessageForReturnDto> Messages { get; set; }
    }
}