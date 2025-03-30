using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        Console.WriteLine(_context.Users.Count());
        if (_context.Users.Any(u => u.Username == user.Username))
        {
            return BadRequest("Username already exists");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
        var dbUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
        if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("supersecret");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                new Claim("id", dbUser.Id.ToString()),
                new Claim("role", dbUser.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token = tokenHandler.WriteToken(token) });
    }
}
