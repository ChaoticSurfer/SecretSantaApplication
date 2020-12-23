using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SecretSantaApplication.Models
{
    public class Room
    {
        public string Creator { get; set; }
        [Key] public string Name { get; set; }
        public string Description { get; set; }
        public string LogoName { get; set; }

        [DisplayName("Upload Room Logo")]
        [NotMapped]
        public IFormFile ImageLogoFile { get; set; }
    }
}