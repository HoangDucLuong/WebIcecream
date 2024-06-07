using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs;
using WebIcecream.Models;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly ProjectDak3Context _context;
        
        public PackageController(ProjectDak3Context context)
        {
            _context = context;
        }

        // GET: api/role
        [HttpGet]
        public ActionResult<IEnumerable<PackageDTO>> GetPackages()
        {
            var roles = _context.Packages
                .Select(r => new PackageDTO
                {
                    PackageID = r.PackageID,
                    PackageName = r.PackageName,
                    Price = r.Price,
                    DurationDays = r.DurationDays
                })
                .ToList();

            return Ok(roles);
        }


        // GET: api/role/{id}
        [HttpGet("{id}")]
        public ActionResult<PackageDTO> GetPackageById(int id)
        {
            var pack = _context.Packages
                .Where(r => r.PackageID == id)
                .Select(r => new PackageDTO
                {
                    PackageID = r.PackageID,
                    PackageName = r.PackageName
                })
                .FirstOrDefault();

            if (pack == null)
            {
                return NotFound();
            }

            return Ok(pack);
        }

        // POST: api/role
        [HttpPost]
        public ActionResult<PackageDTO> CreatePackage(PackageDTO packageDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var package = new Package
            {
                PackageName = packageDTO.PackageName
            };

            _context.Packages.Add(package);
            _context.SaveChanges();

            packageDTO.PackageID = package.PackageID; // Assign the generated RoleId to the DTO

            return CreatedAtAction(nameof(GetPackageById), new { id = packageDTO.PackageID }, packageDTO);
        }

        // PUT: api/role/{id}
        [HttpPut("{id}")]
        public IActionResult UpdatePackage(int id, PackageDTO packageDTO)
        {
            if (id != packageDTO.PackageID)
            {
                return BadRequest();
            }

            var pack = _context.Packages.Find(id);

            if (pack == null)
            {
                return NotFound();
            }

            pack.PackageName = packageDTO.PackageName;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/role/{id}
        [HttpDelete("{id}")]
        public IActionResult DeletePackage(int id)
        {
            var pack = _context.Packages.Find(id);

            if (pack == null)
            {
                return NotFound();
            }

            _context.Packages.Remove(pack);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
