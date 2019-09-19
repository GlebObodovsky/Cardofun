using System;
using System.Collections.Generic;
using Cardofun.Core.Enums;

namespace Cardofun.Domain.Models
{
    public class User
    {
        public Int32 Id { get; set; }
        public String Login { get; set; }
        public String Name { get; set; }
        public DateTime BirthDate { get; set; }
        public Sex Sex { get; set; }
        public Int32 CityId { get; set; }
        public virtual City City { get; set; }
        public String Introduction { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public Byte[] PasswordHash { get; set; }
        public Byte[] PasswordSalt { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<LanguageSpeakingLevel> LanguagesTheUserSpeaks { get; set; }
        public virtual ICollection<LanguageLearningLevel> LanguagesTheUserLearns { get; set; }

        public User()
        {
            LanguagesTheUserSpeaks = new List<LanguageSpeakingLevel>();
            LanguagesTheUserLearns = new List<LanguageLearningLevel>();
        }
    }
}