using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineVotingSystem.Models;
using OnlineVotingSystem.Models.Entities;
using OnlineVotingSystem.Models.Entities.Authentication;
using OnlineVotingSystem.Models.Helper;
using Org.BouncyCastle.Crypto.Generators;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================
// SERVICES
// =======================

builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<databaseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Helpers
builder.Services.AddScoped<EmailHelper>();

// =======================
// JWT AUTHENTICATION
// =======================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "http://localhost:5106/",
            ValidAudience = "http://localhost:5106/",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("OnlineVotingSystemSecretKey123456789")
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
// Seed database (including admin user with HASHED password)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<databaseContext>();

    try
    {   
        context.Database.Migrate();
        Console.WriteLine("Database migrated successfully.");

        // Seed Admin User if not exists
        string adminEmail = "admin@gmail.com";
        if (!context.Users.Any(u => u.Email == adminEmail && u.Role == "Admin"))
        {
            var adminUser = new User
            {
                Email = adminEmail,
                // ? HASH the admin password!
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                Role = "Admin"
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
            Console.WriteLine("Admin user created with hashed password.");
        }
        else
        {
            Console.WriteLine("Admin user already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during database initialization: {ex.Message}");
    }
}

// =======================
// MIDDLEWARE
// =======================

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// =======================
// ROUTING
// =======================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Voters}/{action=Login}/{id?}"
);

app.Run();
