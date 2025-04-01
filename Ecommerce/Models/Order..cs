using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Order
    {
        [Required]
        public int Id { get; set; }
        public int Count { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // [Required]
        // Refers to the navigation property
        // public List<User> UserId { get; set; } = new List<User>();
        /*[Required]
        [ForeignKey("ProductId")]  // Refers to the navigation property
        public int ProductId { get; set; }
        public Order(User UserId) {
            this.UserId.Add(UserId);
        }

        /*public void addUser(User user)
        {
            this.UserId.Add(user);
        }*/

    }
}
