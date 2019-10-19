using System;
using System.ComponentModel.DataAnnotations;

namespace Cardofun.Interfaces.DTOs
{
    /// <summary>
    /// An instance of a user for login actions
    /// </summary>
    public class UserForLoginDto
    {
        /// <summary>
        /// User login
        /// </summary>
        /// <value></value>
        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "You must specify login between 4 and 12")]
        public String Login { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        /// <value></value>
        [Required]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 12")]
        public String Password { get; set; }
    }
}