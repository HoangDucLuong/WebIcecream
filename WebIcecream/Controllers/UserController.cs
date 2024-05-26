﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIcecream.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ProjectDak3Context _context;
        public UserController(ProjectDak3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users
                .Select(m => new UserDTO
                {
                    UserId = m.UserId,
                    FullName = m.FullName,
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                    Gender = m.Gender,
                    Dob = m.Dob,
                    Address = m.Address,
                    PaymentStatus = m.PaymentStatus,
                    RegistrationDate = m.RegistrationDate,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(int userId)
        {
            var user = await _context.Users
                .Where(m => m.UserId == userId)
                .Select(m => new UserDTO
                {
                    UserId = m.UserId,
                    FullName = m.FullName,
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                    Gender = m.Gender,
                    Dob = m.Dob,
                    Address = m.Address,
                    PaymentStatus = m.PaymentStatus,
                    RegistrationDate = m.RegistrationDate,
                    IsActive = m.IsActive
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        

        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDto)
        {
            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Gender = userDto.Gender,
                Dob = userDto.Dob,
                Address = userDto.Address,
                PaymentStatus = userDto.PaymentStatus,
                RegistrationDate = userDto.RegistrationDate,
                IsActive = userDto.IsActive
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDto.UserId = user.UserId;

            return CreatedAtAction(nameof(GetUser), new { id = userDto.UserId }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO userDto)
        {
            if (id != userDto.UserId)
            {
                return BadRequest();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Gender = userDto.Gender;
            user.Dob = userDto.Dob;
            user.Address = userDto.Address;
            user.PaymentStatus = userDto.PaymentStatus;
            user.RegistrationDate = userDto.RegistrationDate;
            user.IsActive = userDto.IsActive;

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userAccounts = await _context.UserAccounts.Where(ua => ua.UserId == id).ToListAsync();
            _context.UserAccounts.RemoveRange(userAccounts);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}