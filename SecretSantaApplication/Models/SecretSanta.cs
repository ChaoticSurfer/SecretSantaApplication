using System.ComponentModel.DataAnnotations;

namespace SecretSantaApplication.Models
{
    public class SecretSanta
    {
        [Key] public string Santa { get; set; }
        public string Target { get; set; }
    }
}