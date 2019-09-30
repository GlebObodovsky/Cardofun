using System;

namespace Cardofun.Interfaces.DTOs
{
    public class CityDto
    {
        public int Id { get; set; }
        /// <summary>
        /// name of the city
        /// </summary>
        /// <value></value>
        public String Name { get; set; }
        /// <summary>
        /// Country ISO code
        /// </summary>
        /// <value></value>
        public String CountryIsoCode { get; set; }
        /// <summary>
        /// Name of the country
        /// </summary>
        /// <value></value>
        public String Country { get; set; }
    }
}