using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebIcecream.DTOs;
using WebIcecream.Models;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MembershipPackagesController : ControllerBase
    {
        private readonly ProjectDak3Context _context;

        public MembershipPackagesController(ProjectDak3Context context)
        {
            _context = context;
        }

        // GET: api/MembershipPackages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembershipPackageDTO>>> GetMembershipPackages()
        {
            return await _context.MembershipPackages
                .Select(package => new MembershipPackageDTO
                {
                    PackageId = package.PackageId,
                    PackageName = package.PackageName,
                    Price = package.Price,
                    DurationDays = (int)package.DurationDays 
                })
                .ToListAsync();
        }

        // GET: api/MembershipPackages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MembershipPackageDTO>> GetMembershipPackage(int id)
        {
            var package = await _context.MembershipPackages.FindAsync(id);

            if (package == null)
            {
                return NotFound();
            }

            var packageDTO = new MembershipPackageDTO
            {
                PackageId = package.PackageId,
                PackageName = package.PackageName,
                Price = package.Price,
                DurationDays = (int)package.DurationDays // Assuming DurationDays is part of Packages table
            };

            return packageDTO;
        }

        // POST: api/MembershipPackages
        [HttpPost]
        public async Task<ActionResult<MembershipPackageDTO>> PostMembershipPackage(MembershipPackageDTO packageDTO)
        {
            var membershippackage = new MembershipPackage
            {
                PackageName = packageDTO.PackageName,
                Price = packageDTO.Price,
                DurationDays = packageDTO.DurationDays // Assuming DurationDays is part of Packages table
            };

            _context.MembershipPackages.Add(membershippackage);
            await _context.SaveChangesAsync();

            packageDTO.PackageId = membershippackage.PackageId;

            return CreatedAtAction(nameof(GetMembershipPackage), new { id = packageDTO.PackageId }, packageDTO);
        }

        // PUT: api/MembershipPackages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembershipPackage(int id, MembershipPackageDTO packageDTO)
        {
            if (id != packageDTO.PackageId)
            {
                return BadRequest();
            }

            var package = await _context.MembershipPackages.FindAsync(id);

            if (package == null)
            {
                return NotFound();
            }

            package.PackageName = packageDTO.PackageName;
            package.Price = packageDTO.Price;
            package.DurationDays = packageDTO.DurationDays; // Assuming DurationDays is part of Packages table

            _context.Entry(package).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(id))
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

        // DELETE: api/MembershipPackages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembershipPackage(int id)
        {
            var package = await _context.MembershipPackages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            _context.MembershipPackages.Remove(package);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PackageExists(int id)
        {
            return _context.MembershipPackages.Any(e => e.PackageId == id);
        }
    }
}
