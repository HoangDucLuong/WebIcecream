using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs; 
using WebIcecream.Models; 

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ProjectDak3Context _context; 

        public RoleController(ProjectDak3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RoleDTO>> GetRoles()
        {
            var roles = _context.Roles
                .Select(r => new RoleDTO
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToList();

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public ActionResult<RoleDTO> GetRoleById(int id)
        {
            var role = _context.Roles
                .Where(r => r.RoleId == id)
                .Select(r => new RoleDTO
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .FirstOrDefault();

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }


        [HttpPost]
        public ActionResult<RoleDTO> CreateRole(RoleDTO roleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new Role
            {
                RoleName = roleDTO.RoleName
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            roleDTO.RoleId = role.RoleId; 

            return CreatedAtAction(nameof(GetRoleById), new { id = roleDTO.RoleId }, roleDTO);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRole(int id, RoleDTO roleDTO)
        {
            if (id != roleDTO.RoleId)
            {
                return BadRequest();
            }

            var role = _context.Roles.Find(id);

            if (role == null)
            {
                return NotFound();
            }

            role.RoleName = roleDTO.RoleName;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRole(int id)
        {
            var role = _context.Roles.Find(id);

            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
