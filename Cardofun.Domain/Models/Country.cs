using System;
using System.Collections.Generic;

namespace Cardofun.Domain.Models
{
    public class Country
    {
        /// <summary>
        /// ISO 3166-1 alpha-2
        /// </summary>
        /// <value></value>
        public String IsoCode { get; set; }
        public String Name { get; set; }
        public String ContinentName { get; set; }
        public virtual Continent Continent { get; set; }

        /// <summary>
        /// Languages that take origin from that country
        /// </summary>
        /// <value></value>
        public virtual ICollection<Language> Languages { get; set; }
        /// <summary>
        /// Cities that country has
        /// </summary>
        /// <value></value>
        public virtual ICollection<City> Cities { get; set; }

        public Country()
        {
            Cities = new List<City>();
            Languages = new List<Language>();
        }
    }
}