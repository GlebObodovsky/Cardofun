using System;

namespace Cardofun.Domain.Models
{
    public class Photo
    {
        public Guid Id { get; set; }
        public String PublicId { get; set; }
        public Int32 UserId { get; set; }
        public User User { get; set; }
        public String Url { get; set; }
        public String Description { get; set; }
        public DateTime DateAdded { get; set; }
        public Boolean IsMain { get; set; }
    }
}