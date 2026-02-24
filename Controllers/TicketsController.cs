using BackendApiExam.Data;
using BackendApiExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApiExam.Controllers
{
    [ApiController]
    [Route("tickets")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "USER,MANAGER")]
        public async Task<IActionResult> Create([FromBody] CreateTicketDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = TicketStatus.OPEN,
                CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var t = await _context.Tickets
                .Include(x => x.CreatedByUser).ThenInclude(u => u.Role)
                .Include(x => x.AssignedToUser).ThenInclude(u => u!.Role)
                .FirstAsync(x => x.Id == ticket.Id);

            return Created("", new
            {
                t.Id,
                t.Title,
                t.Description,
                status = t.Status.ToString(),
                priority = t.Priority.ToString(),
                created_by = new { t.CreatedByUser.Id, t.CreatedByUser.Name, Role = t.CreatedByUser.Role.Name },
                t.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            IQueryable<Ticket> query = _context.Tickets
                .Include(t => t.CreatedByUser).ThenInclude(u => u.Role)
                .Include(t => t.AssignedToUser).ThenInclude(u => u!.Role);

            if (role == "SUPPORT")
            {
                query = query.Where(t => t.AssignedTo == userId);
            }
            else if (role == "USER")
            {
                query = query.Where(t => t.CreatedBy == userId);
            }

            var tickets = await query.ToListAsync();

            var data = tickets.Select(t => new {
                t.Id,
                t.Title,
                t.Description,
                status = t.Status.ToString(),
                priority = t.Priority.ToString(),
                created_by = t.CreatedByUser == null ? null : new { t.CreatedByUser.Id, t.CreatedByUser.Name },
                assigned_to = t.AssignedToUser == null ? null : new { t.AssignedToUser.Id, t.AssignedToUser.Name },
                t.CreatedAt
            });

            return Ok(data);
        }

        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "MANAGER,SUPPORT")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignDTO dto)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound("Ticket not found");

            ticket.AssignedTo = dto.UserId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Assigned" });
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "MANAGER,SUPPORT")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO dto)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound("Ticket not found");

            // check if allowed
            bool ok = false;
            if (ticket.Status == TicketStatus.OPEN && dto.Status == TicketStatus.IN_PROGRESS) ok = true;
            else if (ticket.Status == TicketStatus.IN_PROGRESS && dto.Status == TicketStatus.RESOLVED) ok = true;
            else if (ticket.Status == TicketStatus.RESOLVED && dto.Status == TicketStatus.CLOSED) ok = true;

            if (!ok) return BadRequest("Invalid move");

            var old = ticket.Status;
            ticket.Status = dto.Status;

            var log = new TicketStatusLog
            {
                TicketId = ticket.Id,
                OldStatus = old,
                NewStatus = dto.Status,
                ChangedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                ChangedAt = DateTime.Now
            };
            _context.TicketStatusLogs.Add(log);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Status updated" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MANAGER")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}
