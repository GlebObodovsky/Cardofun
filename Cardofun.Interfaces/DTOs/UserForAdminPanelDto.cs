using System;
using System.Collections.Generic;

namespace Cardofun.Interfaces.DTOs
{
    public class UserForAdminPanelDto
    {
        public Int32 Id { get; set; }
        public String UserName { get; set; }
        public IEnumerable<String> Roles { get; set; }
    }
}