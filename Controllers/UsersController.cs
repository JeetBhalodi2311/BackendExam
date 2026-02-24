using BackendApiExam.Data;
using BackendApiExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApiExam.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Roles = "MANAGER")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return BadRequest(new { message = "Email already exists" });

            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null)
                return BadRequest(new { message = "Invalid role ID" });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            return Created("", new
            {
                user.Id,
                user.Name,
                user.Email,
                role = new { user.Role.Id, user.Role.Name },
                user.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    role = new { u.Role.Id, u.Role.Name },
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
