using System;
using System.Collections.Generic;

namespace Cardofun.Domain.Models
{
    public class Language
    {
        /// <summary>
        /// Language name
        /// </summary>
        /// <value></value>
        public String Name { get; set; }
        /// <summary>
        /// Stands for showing flag icon on UI
        /// </summary>
        /// <value></value>
        public String CountryOfOriginCode { get; set; }
        /// <summary>
        /// Country the language originates from
        /// </summary>
        /// <value></value>
        public virtual Country CountryOfOrigin { get; set; }

        public virtual ICollection<LanguageSpeakingLevel> LanguageSpeakingLevels { get; set; }
        public virtual ICollection<LanguageLearningLevel> LanguageLearningLevels { get; set; }

        public Language()
        {
            LanguageSpeakingLevels = new List<LanguageSpeakingLevel>();
            LanguageLearningLevels = new List<LanguageLearningLevel>();
        }
    }
}