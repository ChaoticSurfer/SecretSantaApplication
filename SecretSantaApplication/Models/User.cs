using System.ComponentModel.DataAnnotations;

namespace SecretSantaApplication.Models
{
    public class User
    {
        [Key] public string EmailAddress { get; set; }
        [DataType(DataType.Password)] 
        public string Password { get; set; }
    }
}