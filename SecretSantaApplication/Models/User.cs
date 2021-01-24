using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSantaApplication.Models
{
    public class User
    {
        [Key] public string EmailAddress { get; set; }

        [Compare("ConfirmPassword", ErrorMessage = "Passwords don't match.")]
        [StringLength(18, MinimumLength = 5, ErrorMessage = "Password min length must at least 6.")]
        [RegularExpression(@"^.*(?=.*[A-Z])(?=.*[0-9])(?=.*).*$",
            ErrorMessage = "Password must contain at least one uppercase character and one number.")]
        public string Password { get; set; }

        [NotMapped] public string ConfirmPassword { get; set; }

        public ICollection<UserToRoom> UserToRooms { get; set; } = new List<UserToRoom>();
    }
}