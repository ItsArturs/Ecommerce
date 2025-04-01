using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    
    // private readonly IConfiguration _configuration;

    /*public AuthController(/*UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager*//*, IConfiguration configuration)*/
    /*{
        /*_userManager = userManager;
        _signInManager = signInManager;*/
        // Configure Authentication & Authorization
        /*var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
        _configuration = configuration;
        Conf
    }*/
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(string Username, string Password, string Role)
    {
        if ((Username == null) || (Password == null) || (Role == null)) { return BadRequest("Fields must be filled!"); }
        if (_context.Users.Any(u => u.Username == Username)) {  return BadRequest("Username already exists"); }

        int lastId = _context.Users.Max(obj => obj.Id);
        Password = BCrypt.Net.BCrypt.HashPassword(Password);

        User user = new(lastId + 1, Username, Password, Role);

        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            return BadRequest("Invalid user! Error msg: " + ex.Message);
        }
        return Ok(new { message = "User registered successfully" });
    }

    /*[HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
        var dbUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
        if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("supersecret12345supersecret12345");
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
    }*/
    [HttpPost("login")]
    public IActionResult Login(string Username, string Password)
    {
        if ((Username == null) || (Password == null)) { return BadRequest("Invalid credentials"); }

        UserLogin dbUser = new(Username, Password, DateTime.UtcNow.AddHours(1));

        _context.UsersLogin.Add(dbUser);

        if (!_context.IsAuthorised()) { return Unauthorized("Invalid credentials"); }

        // Read JWT secret from configuration
        // var jwtSettings = _configuration.GetSection("Jwt");
        /*var key = Encoding.UTF8.GetBytes("supersecret12345supersecret12345");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()), // User ID
            new Claim(ClaimTypes.Role, dbUser.Role) // Use ClaimTypes.Role for role-based authorization
        }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "yourdomain.com", // Set Issuer
            Audience = "yourdomain.com", // Set Audience
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));*/

        // return Ok(new { token = Token });
        return Ok(new { msg = "Loged in succesfuly!" });
    }
}
