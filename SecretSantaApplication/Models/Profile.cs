using System.ComponentModel.DataAnnotations;

namespace SecretSantaApplication.Models
{
    public class Profile
    {
        [Key] public string EmailAddress { get; set; }
        [DataType(DataType.Password)] public string BirthDate { get; set; }
        public string LetterToSecretSanta { get; set; }
    }
}