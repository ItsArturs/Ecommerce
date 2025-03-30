using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult PlaceOrder([FromBody] int[] productIds)
    {
        var userId = int.Parse(User.FindFirst("id").Value);
        var order = new Order { UserId = userId };
        _context.Orders.Add(order);
        _context.SaveChanges();

        return Ok(new { message = "Order placed successfully" });
    }
}
