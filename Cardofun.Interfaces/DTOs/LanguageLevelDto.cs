using System;
using Cardofun.Core.Enums;

namespace Cardofun.Interfaces.DTOs
{
    public class LanguageLevelDto
    {
        /// <summary>
        /// Language code
        /// </summary>
        /// <value></value>
        public String Code { get; set; }        
        /// <summary>
        /// Language name
        /// </summary>
        /// <value></value>
        public String Name { get; set; }
        /// <summary>
        /// Level of user's language proficiency
        /// </summary>
        /// <value></value>
        public LevelOfSpeaking LevelOfSpeaking { get; set; }
    }
}