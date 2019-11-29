using System;

namespace Cardofun.Interfaces.DTOs
{
    public class UserPhotoForReturnDto: UserPhotoDto
    {
        /// <summary>
        /// Id that is ised for third party services
        /// </summary>
        public String PublicId { get; set; }
    }
}