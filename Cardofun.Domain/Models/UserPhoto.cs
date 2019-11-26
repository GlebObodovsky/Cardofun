using System;

namespace Cardofun.Domain.Models
{
    public class UserPhoto
    {
        public Guid Id { get; set; }
        public Int32 UserId { get; set; }
        public virtual User User { get; set; }
        public Guid PhotoId { get; set; }
        public virtual Photo Photo { get; set; }
        public String Description { get; set; }
        public Boolean IsMain { get; set; }
        public DateTime DateAdded { get; set; }
    }
}