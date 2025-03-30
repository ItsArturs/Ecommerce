using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Ecommerce.Models  // Pievieno šo
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "admin" vai "user"

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
