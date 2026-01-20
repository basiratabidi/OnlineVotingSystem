using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineVotingSystem.Models;
using OnlineVotingSystem.Models.Entities;
using OnlineVotingSystem.Models.Entities.Authentication;
using OnlineVotingSystem.Models.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineVotingSystem.Controllers
{
    public class VotersController : Controller
    {
        private readonly databaseContext _context;
        public VotersController(databaseContext context)
        {
            _context = context;
        }

                       //  REGISTER 
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(VoterRegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password), //hashed password
                Role = "Voter"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var voter = new Voter
            {
                Name = model.Name,
                Email = model.Email,
                password= BCrypt.Net.BCrypt.HashPassword(model.Password),
                UserId = user.UserId,
                HasVoted = false
            };

            _context.Voters.Add(voter);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

                                 //  LOGIN 
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(VoterLoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loggedInUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (loggedInUser == null)
            {
                ModelState.AddModelError("", "Incorrect Email!");
                return View(model);
            }

            // plain password vs stored hash
            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                model.Password,
                loggedInUser.HashedPassword
            );

            if (!passwordValid)
            {
                ModelState.AddModelError("", "Incorrect Password!");
                return View(model);
            }

            Voter voter = null;
            if (loggedInUser.Role == "Voter")
            {
                voter = await _context.Voters
                    .FirstOrDefaultAsync(v => v.UserId == loggedInUser.UserId);

                if (voter == null)
                {
                    ModelState.AddModelError("", "Voter profile not found.");
                    return View(model);
                }
            }

            var token = GenerateToken(loggedInUser, voter);

            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return loggedInUser.Role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Dashboard", "Voters");
        }

                            //  DASHBOARD 
        [Authorize(Roles = "Voter")]
        public IActionResult Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var voter = _context.Voters.Include(v => v.User).FirstOrDefault(v => v.UserId == userId);
            if (voter == null) return RedirectToAction("Login");
            return View(voter);
        }

                         //  PROFILE 
        [Authorize(Roles = "Voter")]
        public IActionResult Profile()
        {
            int voterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Include the related User entity
            var voter = _context.Voters
                .Include(v => v.User)
                .FirstOrDefault(v => v.UserId == voterId);

            if (voter == null) return RedirectToAction("Login");

            return View(voter);
        }


                     //  SELECT ELECTION 
        [Authorize(Roles = "Voter")]
        public IActionResult SelectElection(string action = null)
        {
            var activeElections = _context.Elections
                .Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now)
                .ToList();
            
            // Also pass all elections for party management
            var allElections = _context.Elections
                .OrderByDescending(e => e.EndDate)
                .ToList();
            
            ViewBag.AllElections = allElections;
            ViewBag.Action = action; // Store which action was requested (AddParty, RegisterCandidate, etc.)
            
            return View(activeElections);
        }

      


                             //  LOGOUT 
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Login");
        }

        //  JWT TOKEN GENERATION 
        private static string GenerateToken(User user, Voter voter = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email)
            };

            if (voter != null)
                claims.Add(new Claim(ClaimTypes.Name, voter.Name));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("OnlineVotingSystemSecretKey123456789"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5106/",
                audience: "http://localhost:5106/",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
