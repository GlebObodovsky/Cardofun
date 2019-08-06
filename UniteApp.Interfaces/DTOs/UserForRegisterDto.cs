using System.ComponentModel.DataAnnotations;

namespace UniteApp.Interfaces.DTOs
{
    /// <summary>
    /// An instance of a user that is about to be registrated
    /// </summary>
    public class UserForRegisterDto
    {
        /// <summary>
        /// New user login
        /// </summary>
        /// <value></value>
        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "You must specify login between 4 and 12")]
        public string Login { get; set; }
        /// <summary>
        /// New user password
        /// </summary>
        /// <value></value>
        [Required]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 12")]
        public string Password { get; set; }
    }
}