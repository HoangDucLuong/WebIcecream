using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using WebIcecream.DTOs;
using WebIcecream.Models;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ProjectDak3Context _context;

        public AuthController(IConfiguration config, ProjectDak3Context context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            // Authenticate user
            var user = _context.UserAccounts.Include(u => u.User).SingleOrDefault(u => u.Username == loginDTO.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                return Unauthorized(); // Unauthorized if user not found or invalid credentials
            }

            // Generate JWT token
            var tokenString = GenerateJWTToken(user);

            // Return token
            return Ok(new { Token = tokenString });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            // Check if username is already taken
            if (await _context.UserAccounts.AnyAsync(u => u.Username == registerDTO.Username))
            {
                return BadRequest("Username is already taken");
            }

            // Hash password using bcrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            // Retrieve membership package information (assuming you have it available)
            var package = await _context.MembershipPackages.FirstOrDefaultAsync(p => p.PackageId == registerDTO.PackageId);

            if (package == null)
            {
                return BadRequest("Invalid PackageId"); // Handle case where package is not found
            }

            // Create new user profile
            var newUser = new User
            {
                FullName = registerDTO.FullName,
                Dob = registerDTO.Dob,
                Address = registerDTO.Address,
                Gender = registerDTO.Gender,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                PaymentStatus = "Pending", // You may set a default payment status
                RegistrationDate = DateTime.Now,
                IsActive = true, // Assuming user is active upon registration
                PackageId = registerDTO.PackageId,
                PackageStartDate = DateTime.Now,
                PackageEndDate = DateTime.Now.AddDays(package.DurationDays) // Set end date based on package duration
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Create new user account with hashed password and default role
            var newUserAccount = new UserAccount
            {
                UserId = newUser.UserId, // Assign UserId from newly created User
                Username = registerDTO.Username,
                Password = hashedPassword,
                RoleId = 1
            };

            _context.UserAccounts.Add(newUserAccount);
            await _context.SaveChangesAsync();

            return StatusCode(201); // Created
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Clear token from session or cookie
            HttpContext.Session.Remove("Token"); // or HttpContext.Response.Cookies.Delete("Token")

            // Return success status code
            return Ok();
        }

        private string GenerateJWTToken(UserAccount userAccount)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userAccount.Username),
                    new Claim("userId", userAccount.UserId.ToString()),
                    new Claim(ClaimTypes.Role, userAccount.RoleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
