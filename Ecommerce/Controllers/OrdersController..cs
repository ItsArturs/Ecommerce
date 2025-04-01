using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using System.Security.Claims;
using System.Data;

[Route("api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public IActionResult PlaceOrder(string product, int count)
    {
        if ((product == null) || (count == 0)) { return BadRequest("Fields must be filled!"); }
        // var userId = int.Parse(User.FindFirst("id").Value);
        // var order = new Order { UserId = userId };
        // var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (!_context.IsAuthorised())
        {
            return Unauthorized("You are not logged in or login has expired!");
        }

        int lastId = _context.Users.Max(obj => obj.Id);
        Order order = new Order();
        order.Id = lastId;
        order.Count = count;

        UserLogin userLogin = new("", "", DateTime.Now);
        try
        {
            userLogin = _context.UsersLogin.FirstOrDefault();
            order.User = _context.Users.FirstOrDefault(p => p.Username == userLogin.Username);
            order.UserId = order.User.Id;
        } catch (Exception ex) { return BadRequest("Invalid"); }

        var prod = _context.Products.FirstOrDefault(pr => pr.Name == product);
        if (prod == null) { return BadRequest("Product not found!"); }

        order.Product = prod;
        order.ProductId = order.Product.Id;

        try
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            return BadRequest("Invalid order! Error msg: " + ex.Message);
        }

        return Ok(new { message = "Order placed successfully!" });
    }
}
