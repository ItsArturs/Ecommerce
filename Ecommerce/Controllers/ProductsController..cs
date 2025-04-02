using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(_context.Products.ToList());
    }

    [HttpPost]
    public IActionResult AddProduct(string Name, decimal price)
    {
        if ((Name == null) || (price == decimal.Zero)) { return BadRequest("No product to add!"); }
        if (!_context.IsAuthorised("Admin")) { return Unauthorized("You are not logged in as Admin or login has expired"); }

        int lastId = _context.Products.Max(obj => obj.Id);
        Product product = new() { Id = lastId + 1, Name = Name, Price = price };

        try
        {
            _context.Products.Add(product);
            _context.SaveChanges();

        } catch (Exception ex) { 
            return BadRequest("Invalid product! Error msg: " + ex.Message); 
        }
        
        return Ok(product);
    }
    [HttpDelete]
    public IActionResult DeletteProduct(string Name) {
        if (Name == null) { return BadRequest("No product to delette!"); }
        if (!_context.IsAuthorised("Admin")) { return Unauthorized("You are not logged in as Admin or login has expired"); }
        Product product;
        try
        {
            product = _context.Products.FirstOrDefault(u => u.Name == Name);
        } catch (Exception ex) { return BadRequest("Can not delette product!"); }
       

        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
        return Ok(Name);
    
    }
}
