﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIcecream.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebIcecream.Models;
using System;

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
                    IsActive = m.PackageEndDate >= DateTime.Now, 
                    PackageId = m.PackageId,
                    PackageName = m.Package != null ? m.Package.PackageName : null,  
                    PackageStartDate = m.PackageStartDate,
                    PackageEndDate = m.PackageEndDate
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
                    IsActive = m.PackageEndDate >= DateTime.Now, 
                    PackageId = m.PackageId,
                    PackageName = m.Package != null ? m.Package.PackageName : null,  
                    PackageStartDate = m.PackageStartDate,
                    PackageEndDate = m.PackageEndDate
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
                IsActive = userDto.PackageEndDate >= DateTime.Now, 
                PackageId = userDto.PackageId,
                PackageStartDate = userDto.PackageStartDate,
                PackageEndDate = userDto.PackageEndDate
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDto.UserId = user.UserId;
            userDto.IsActive = user.IsActive; 
            userDto.PackageName = user.Package != null ? user.Package.PackageName : null;
            userDto.PackageStartDate = user.PackageStartDate;
            userDto.PackageEndDate = user.PackageEndDate;

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
            user.IsActive = userDto.PackageEndDate >= DateTime.Now; 
            user.PackageId = userDto.PackageId;
            user.PackageStartDate = userDto.PackageStartDate;
            user.PackageEndDate = userDto.PackageEndDate;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> SearchUsersByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var users = await _context.Users
                .Where(b => b.FullName.Contains(name))
                .Select(b => new UserDTO
                {
                    FullName = b.FullName,
                    Email = b.Email,
                    PhoneNumber = b.PhoneNumber,
                    Gender = b.Gender,
                    Dob = b.Dob,
                    Address = b.Address,
                    PaymentStatus = b.PaymentStatus,
                    RegistrationDate = b.RegistrationDate,
                    IsActive = b.PackageEndDate >= DateTime.Now, 
                    PackageId = b.PackageId,
                    PackageStartDate = b.PackageStartDate,
                    PackageEndDate = b.PackageEndDate
                })
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No user found with the provided name.");
            }

            return Ok(users);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> RenewMembership(int userId, RenewMembershipDTO model)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.PackageId = model.PackageId;
            user.PackageStartDate = DateTime.UtcNow; 
            user.PackageEndDate = DateTime.UtcNow.AddYears(1); 

            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
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

        [HttpGet("{username}")]
        public async Task<ActionResult<bool>> IsActive(string username)
        {
            var user = await _context.UserAccounts
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.User == null)
            {
                return NotFound();
            }

            bool isActive = user.User.PackageEndDate >= DateTime.Now;
            return Ok(isActive);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
