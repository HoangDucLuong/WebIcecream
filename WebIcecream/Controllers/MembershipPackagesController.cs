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
                DurationDays = (int)package.DurationDays
            };

            return packageDTO;
        }

        [HttpPost]
        public async Task<ActionResult<MembershipPackageDTO>> PostMembershipPackage(MembershipPackageDTO packageDTO)
        {
            var membershippackage = new MembershipPackage
            {
                PackageName = packageDTO.PackageName,
                Price = packageDTO.Price,
                DurationDays = packageDTO.DurationDays 
            };

            _context.MembershipPackages.Add(membershippackage);
            await _context.SaveChangesAsync();

            packageDTO.PackageId = membershippackage.PackageId;

            return CreatedAtAction(nameof(GetMembershipPackage), new { id = packageDTO.PackageId }, packageDTO);
        }


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
            package.DurationDays = packageDTO.DurationDays; 

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembershipPackageDTO>>> SearchMembershipsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var memberships = await _context.MembershipPackages
                .Where(b => b.PackageName.Contains(name))
                .Select(b => new MembershipPackageDTO
                {
                    PackageId = b.PackageId,
                    PackageName = b.PackageName,
                    Price = b.Price,
                    DurationDays = b.DurationDays           
                })
                .ToListAsync();

            if (memberships == null || memberships.Count == 0)
            {
                return NotFound("No MembershipPackage found with the provided name.");
            }

            return Ok(memberships);
        }


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
