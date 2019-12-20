using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Cardofun.Domain.Models
{
    public class Role: IdentityRole<Int32> 
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public Role()
        {
            UserRoles = new List<UserRole>();
        }
    }
}