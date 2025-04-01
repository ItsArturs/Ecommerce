using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public IActionResult PlaceOrder([FromBody] string product, int count, int user)
    {

        // var userId = int.Parse(User.FindFirst("id").Value);
        // var order = new Order { UserId = userId };
        var orderedOrders = _context.Orders.OrderBy(obj => obj.Id).ToList();
        Order order = new Order();
        order.Count = count;

        var orderedUsers = _context.Users.OrderBy(obj => obj.Id).ToList();
        order.User = orderedUsers.Last();
        order.UserId = order.User.Id;

        order.Product = _context.Products.FirstOrDefault(pr => pr.Name == product);
        order.ProductId = order.Product.Id;

        _context.Orders.Add(order);
        _context.SaveChanges();

        return Ok(new { message = "Order placed successfully" });
    }
}
