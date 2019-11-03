using System;

namespace Cardofun.Interfaces.DTOs
{
    public class CountryDto
    {
        /// <summary>
        /// ISO 3166-1 alpha-2
        /// </summary>
        /// <value></value>
        public String IsoCode { get; set; }
        public String Name { get; set; }
        public String ContinentName { get; set; }
    }
}