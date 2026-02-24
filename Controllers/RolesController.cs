using BackendApiExam.Data;
using BackendApiExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApiExam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "MANAGER")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RolesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // get all roles from database
            var roles = await _db.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleDTO role)
        {
            // check if role name already exists
            var exists = await _db.Roles.AnyAsync(r => r.Name == role.Name);
            if (exists)
            {
                return BadRequest("Role already exists");
            }

            var r = new Role { Name = role.Name };

            _db.Roles.Add(r);
            await _db.SaveChangesAsync();

            return Ok(r);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RoleDTO role)
        {
            var r = await _db.Roles.FindAsync(id);
            if (r == null) return NotFound("Role not found");

            r.Name = role.Name;
            await _db.SaveChangesAsync();

            return Ok(r);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.Roles.FindAsync(id);
            if (r == null) return NotFound("Role not found");

            _db.Roles.Remove(r);
            await _db.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}