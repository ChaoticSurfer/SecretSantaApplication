using System;
using System.Collections.Generic;
using SecretSantaApplication.Models;


namespace SecretSantaApplication.Models
{
    public class UserToRoom
    {
        public DateTime JoinDate { get; set; }
        public string Name { get; set; }
        public Room Room { get; set; }
        public string EmailAddress { get; set; }
        public User User { get; set; }
    }
}