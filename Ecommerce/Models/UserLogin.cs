using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Models  // Pievieno šo
{
    public class UserLogin
    {
        [Key]
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime Expires { get; set; }

        public UserLogin(string username, string password, DateTime expires)
        {
            Username = username;
            Password = password;
            Expires = expires;
        }
    }
}

