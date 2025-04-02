using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce.Models  // Pievieno šo
{
    public class User
    {

        [Required]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "admin" vai "user"
        [Required]
        public List<Order> Orders { get; set; } = new List<Order>();
        public LoginInfo? LoginInfo { get; set; }

        public User(int Id, string Username, string Password, string Role) {
            this.Id = Id;
            this.Username = Username;
            this.Password = Password;
            this.Role = Role;
        }

        public User(string Username, string Password)
        {
            Id = -1;
            this.Username = Username;
            this.Password = Password;
            Role = "defoult";
        }
    }
}
