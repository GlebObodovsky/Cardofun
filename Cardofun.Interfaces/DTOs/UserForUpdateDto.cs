using System;
using System.Collections.Generic;
using Cardofun.Core.Enums;

namespace Cardofun.Interfaces.DTOs
{
    public class UserForUpdateDto
    {
        public String Name { get; set; }
        public DateTime BirthDate { get; set; }
        public Sex Sex { get; set; }
        public CityDto City { get; set; }
        public String Introduction { get; set; }
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