using System.ComponentModel.DataAnnotations;

namespace SecretSantaApplication.Models
{
    public class Profile
    {
        [Key] public string EmailAddress { get; set; }
        public string BirthDate { get; set; }
        public string LetterToSecretSanta { get; set; }
    }
}