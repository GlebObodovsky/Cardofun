using System;
using System.Collections.Generic;

namespace Cardofun.Domain.Models
{
    public class Continent
    {
        public String Name { get; set; }
        public virtual ICollection<Country> Countries { get; set; }

        public Continent()
        {
            Countries = new List<Country>();
        }
    }
}