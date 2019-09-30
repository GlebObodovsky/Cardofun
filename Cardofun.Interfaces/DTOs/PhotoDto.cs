using System;

namespace Cardofun.Interfaces.DTOs
{
    public class PhotoDto
    {
        public Guid Id { get; set; }
        public String Url { get; set; }
        public String Description { get; set; }
        public DateTime DateAdded { get; set; }
        public Boolean IsMain { get; set; }
    }
}