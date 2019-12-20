using System;
using Microsoft.AspNetCore.Identity;

namespace Cardofun.Domain.Models
{
    public class UserRole: IdentityUserRole<Int32> 
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}