using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.Models;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=Ecommerce; Trusted_Connection=true; Trust Server Certificate=true; MultipleActiveResultSets=true; Integrated Security=true;"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Authentication & Authorization
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
// builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));


var app = builder.Build();

app.UseAuthentication(); // Enable Authentication 
app.UseAuthorization();  // Enable Authorization 

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Pievieno SQL Server savienojumu


// Pievieno autentifik?ciju ar JWT
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecret12345supersecret12345"))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();*/

app.Run();

// ?? Datu b?zes konteksts
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<UserLogin> UsersLogin { get; set; }
    // public User LogedInUser { get; set; } = null!;
    // public DateTime? LoginExpiresAt { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*modelBuilder.Entity<User>()
            .HasOne(user => user.u)
            .WithMany(Orders => Orders.User)// orders => Users.Append(new User(6, "string1", "string", "Admin"))) // Add(new User(6, "string1", "string", "Admin"))
            .HasForeignKey(user => user.Orders)
            .OnDelete(DeleteBehavior.Cascade);*/
        modelBuilder.Entity<User>()
           .HasMany(e => e.Orders)
           .WithOne(e => e.User)
           .HasForeignKey(e => e.UserId)
           .IsRequired();
        modelBuilder.Entity<Product>()
           .HasMany(e => e.Orders)
           .WithOne(e => e.Product)
           .HasForeignKey(e => e.ProductId)
           .IsRequired();
    }
    public bool IsAuthorised()
    {
        UserLogin userLogin = new("", "", DateTime.Now);
        try
        {
            userLogin = UsersLogin.FirstOrDefault();
        } catch (Exception ex) { return false; }

        var dbUser = Users.FirstOrDefault(u => u.Username == userLogin.Username);
        if ((dbUser == null) || (!BCrypt.Net.BCrypt.Verify(userLogin.Password, dbUser.Password))) { return false; }

        if(userLogin.Expires < DateTime.UtcNow) { return false; }

        return true;
    }

    public bool IsAdmin()
    {
        UserLogin userLogin = new("", "", DateTime.Now);
        try
        {
            userLogin = UsersLogin.FirstOrDefault();
        }
        catch (Exception ex) { return false; }
        var dbUser = Users.FirstOrDefault(u => u.Username == userLogin.Username);

        if (IsAuthorised()) { return dbUser.Role.Equals("Admin"); } else { return false; }
    }
}


// ?? Mode?i
/*public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}*/
