using System;
using Cardofun.Core.Enums;

namespace Cardofun.Core.ApiParameters
{
    /// <summary>
    /// Api parameters that are needed for user filtering
    /// </summary>
    public class UserParams: PaginationParams
    {
        /// <summary>
        /// Gender of users by which to search. Both if null
        /// </summary>
        /// <value></value>
        public Sex? Sex { get; set; }
        /// <summary>
        /// Minimum age of the needed users
        /// </summary>
        /// <value></value>
        public Int32? AgeMin { get; set; }
        /// <summary>
        /// Maximum age of the needed user
        /// </summary>
        /// <value></value>
        public Int32? AgeMax { get; set; }
        /// <summary>
        /// Language that wanted to be spoken by searching users
        /// </summary>
        /// <value></value>
        public String LanguageSpeakingCode { get; set; }
        /// <summary>
        /// Language that wanted to be studied by searching users
        /// </summary>
        /// <value></value>
        public String LanguageLearningCode { get; set; }
        /// <summary>
        /// Search by users from that city
        /// </summary>
        /// <value></value>
        public int? CityId { get; set; }
        /// <summary>
        /// Search by users from that country
        /// </summary>
        /// <value></value>
        public String CountryIsoCode { get; set; }
    }
}