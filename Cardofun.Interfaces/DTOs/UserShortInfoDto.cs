using System;
using Cardofun.Core.Enums;

namespace Cardofun.Interfaces.DTOs
{
    public class UserShortInfoDto
    {
        public Int32 Id { get; set; }
        public String Login { get; set; }
        public String Name { get; set; }
        public Int32 Age { get; set; }
        public Sex Sex { get; set; }
        public Int32 CityId { get; set; }
        public DateTime Created { get; set; }
        /// <summary>
        /// Url of the main user's photo
        /// </summary>
        /// <value></value>
        public String PhotoUrl { get; set; }
    }
}