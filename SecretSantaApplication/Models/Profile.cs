using System.ComponentModel.DataAnnotations;

namespace SecretSantaApplication.Models
{
    public class Profile
    {
        [Key] public string EmailAddress { get; set; }
        [DataType(DataType.Password)] public int Age { get; set; }
        public string Wishes { get; set; }
    }
}