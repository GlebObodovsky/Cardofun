using System;
using Cardofun.Core.Enums;

namespace Cardofun.Domain.Models
{
    public abstract class LanguageLevel
    {
        public Int32 UserId { get; set; }
        public virtual User User { get; set; }
        public String LanguageCode { get; set; }
        public virtual Language Language { get; set; }
        /// <summary>
        /// Level of user's language proficiency
        /// </summary>
        /// <value></value>
        public LevelOfSpeaking LevelOfSpeaking { get; set; }
    }
}