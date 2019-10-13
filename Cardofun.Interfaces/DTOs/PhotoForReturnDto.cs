using System;

namespace Cardofun.Interfaces.DTOs
{
    public class PhotoForReturnDto: PhotoDto
    {
        /// <summary>
        /// Id that is ised for third party services
        /// </summary>
        public String PublicId { get; set; }
    }
}