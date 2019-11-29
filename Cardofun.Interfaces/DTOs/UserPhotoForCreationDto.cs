using System;
using Microsoft.AspNetCore.Http;

namespace Cardofun.Interfaces.DTOs
{
    public class UserPhotoForCreationDto
    {
        /// <summary>
        /// Id that is ised for third party services
        /// </summary>
        public String PublicId { get; set; }
        public String Url { get; set; }
        public String Description { get; set; }
        public DateTime DateAdded { get; set; }
        public IFormFile File { get; set; }
    }
}