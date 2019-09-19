using System;
using System.Collections.Generic;

namespace Cardofun.Domain.Models
{
    public class City
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String CountryIsoCode { get; set; }
        public virtual Country Country { get; set; }
        
        public virtual ICollection<User> Users { get; set; }

        public City()
        {
            Users = new List<User>();
        }
    }
}