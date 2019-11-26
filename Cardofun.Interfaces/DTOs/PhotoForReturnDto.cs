using System;

namespace Cardofun.Interfaces.DTOs
{
    public class PhotoForReturnDto: UserPhotoDto
    {
        /// <summary>
        /// Id that is ised for third party services
        /// </summary>
        public String PublicId { get; set; }
    }
}