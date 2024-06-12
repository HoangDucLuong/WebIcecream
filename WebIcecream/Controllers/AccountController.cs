using WebIcecream.DTOs;
using WebIcecream.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ProjectDak3Context _context;

        public AccountController(IConfiguration config, ProjectDak3Context context)
        {
            _config = config;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> ChangePasswordByUsername([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == changePasswordDTO.Username);

                if (userAccount == null || !BCrypt.Net.BCrypt.Verify(changePasswordDTO.OldPassword, userAccount.Password))
                {
                    return Unauthorized("Invalid old password"); 
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.NewPassword);
                userAccount.Password = hashedPassword;

                await _context.SaveChangesAsync();

                var tokenString = GenerateJWTToken(userAccount);

                return Ok(new { Token = tokenString }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        private string GenerateJWTToken(UserAccount userAccount)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userAccount.Username.ToString()), 
                    new Claim("userId", userAccount.UserId.ToString()) 
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
