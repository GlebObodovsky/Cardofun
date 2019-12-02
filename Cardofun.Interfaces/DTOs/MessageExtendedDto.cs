using System;

namespace Cardofun.Interfaces.DTOs
{
    /// <summary>
    /// Represents a last message in unser container
    /// </summary>
    public class MessageExtendedDto
    {
        public Guid Id { get; set; }
        public Int32 SenderId { get; set; }
        public String SenderName { get; set; }
        public String SenderPhotoUrl { get; set; }
        public Int32 RecipientId { get; set; }
        public String RecipientName { get; set; }
        public String RecipientPhotoUrl { get; set; }
        public String Text { get; set; }
        public String PhotoUrl { get; set; }
        public DateTime SentAt { get; set; }
        public Boolean IsRead => ReadAt.HasValue;
        public DateTime? ReadAt { get; set; }
    }
}