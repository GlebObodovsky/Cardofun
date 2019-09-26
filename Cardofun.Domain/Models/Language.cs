using System;
using System.Collections.Generic;

namespace Cardofun.Domain.Models
{
    public class Language
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
        public virtual ICollection<LanguageSpeakingLevel> LanguageSpeakingLevels { get; set; }
        public virtual ICollection<LanguageLearningLevel> LanguageLearningLevels { get; set; }

        public Language()
        {
            LanguageSpeakingLevels = new List<LanguageSpeakingLevel>();
            LanguageLearningLevels = new List<LanguageLearningLevel>();
        }
    }
}