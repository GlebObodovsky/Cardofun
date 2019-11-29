using System;

namespace Cardofun.Interfaces.DTOs
{
    public class MessageForReturnDto
    {
        public Guid Id { get; set; }
        public Int32 SenderId { get; set; }
        public Int32 RecipientId { get; set; }
        public String Text { get; set; }
        public String PhotoUrl { get; set; }
        public Boolean IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }
    }
}