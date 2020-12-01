using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSantaApplication.Models
{
    public class User
    {
        [Key] public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}