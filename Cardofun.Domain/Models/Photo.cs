using System;

namespace Cardofun.Domain.Models
{
    public class Photo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Id for third party image storages
        /// </summary>
        /// <value></value>
        public String PublicId { get; set; }
        public String Url { get; set; }
        public virtual UserPhoto UserPhoto { get; set; }
        public virtual Message Message { get; set; }
    }
}