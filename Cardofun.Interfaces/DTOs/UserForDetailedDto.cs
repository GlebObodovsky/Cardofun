using System;
using System.Collections.Generic;
using Cardofun.Core.Enums;

namespace Cardofun.Interfaces.DTOs
{
    public class UserForDetailedDto
    {
        public Int32 Id { get; set; }
        public String Login { get; set; }
        public String Name { get; set; }
        public Int32 Age { get; set; }
        public DateTime BirthDate { get; set; }
        public Sex Sex { get; set; }
        public CityDto City { get; set; }
        public String Introduction { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        /// <summary>
        /// Url of the main user's photo
        /// </summary>
        /// <value></value>
        public String PhotoUrl { get; set; }
        /// <summary>
        /// All user's photos
        /// </summary>
        /// <value></value>
        public IEnumerable<UserPhotoDto> Photos { get; set; }
        /// <summary>
        /// Collection of languages the user speaks and want to help out with
        /// </summary>
        /// <value></value>
        public IEnumerable<LanguageLevelDto> LanguagesTheUserSpeaks { get; set; }
        /// <summary>
        /// Collection of languages the user learns and seeks for helping with
        /// </summary>
        /// <value></value>
        public IEnumerable<LanguageLevelDto> LanguagesTheUserLearns { get; set; }
    }
}