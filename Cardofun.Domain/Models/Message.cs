using System;

namespace Cardofun.Domain.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public Int32 SenderId { get; set; }
        public virtual User Sender { get; set; }
        public Int32 RecipientId { get; set; }
        public virtual User Recipient { get; set; }
        public String Text { get; set; }
        public Guid? PhotoId { get; set; }
        public virtual Photo Photo { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }
        public Boolean SenderDeleted { get; set; }
        public Boolean RecipientDeleted { get; set; }
    }
}