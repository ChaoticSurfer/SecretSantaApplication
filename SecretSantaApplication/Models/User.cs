using System.ComponentModel.DataAnnotations;

namespace SecretSantaApplication.Models
{
    public class User
    {
        [Key] public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression("([A-Za-z]+[0-9]|[0-9]+[A-Za-z])[A-Za-z0-9]*", ErrorMessage = "Passwords must contain at least one number and character")]
        [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        
        public string Password { get; set; }
    }
}