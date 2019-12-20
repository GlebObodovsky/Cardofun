using System;
using System.ComponentModel.DataAnnotations;
using Cardofun.Core.Enums;

namespace Cardofun.Interfaces.DTOs
{
    /// <summary>
    /// An instance of a user that is about to be registrated
    /// </summary>
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "You must specify user name between 4 and 12")]
        public String UserName { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public Sex Sex { get; set; }
        [Required]
        public Int32 CityId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String Email { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "You must specify password of at lest 6 characters")]
        public String Password { get; set; }
    }
}