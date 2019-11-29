using System;
using Microsoft.AspNetCore.Http;

namespace Cardofun.Interfaces.DTOs
{
    public class MessageForCreationDto
    {
        public Int32 SenderId { get; set; }
        public Int32 RecipientId { get; set; }
        public String Text { get; set; }
        /// <summary>
        /// Uploading file (photo)
        /// </summary>
        /// <value></value>
        public IFormFile File { get; set; }
        /// <summary>
        /// Photo Id's
        /// </summary>
        /// <value></value>
        public GlobalPhotoIdentifiersDto Photo { get; set; }
    }
}