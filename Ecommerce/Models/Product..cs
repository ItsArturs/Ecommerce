using System.ComponentModel.DataAnnotations;
namespace Ecommerce.Models
{
    public class Product
    {
        [Required] 
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}