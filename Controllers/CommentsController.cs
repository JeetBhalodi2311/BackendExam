using BackendApiExam.Data;
using BackendApiExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApiExam.Controllers
{
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("tickets/{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CommentDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound("Ticket not found");

            // check access manually
            bool allow = false;
            if (role == "MANAGER") allow = true;
            else if (role == "SUPPORT" && ticket.AssignedTo == userId) allow = true;
            else if (role == "USER" && ticket.CreatedBy == userId) allow = true;

            if (!allow) return StatusCode(403, "Forbidden");

            var comment = new TicketComment
            {
                TicketId = id,
                UserId = userId,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now
            };

            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();

            var c = await _context.TicketComments
                .Include(x => x.User).ThenInclude(u => u.Role)
                .FirstAsync(x => x.Id == comment.Id);

            return Created("", new {
                c.Id,
                c.Comment,
                User = new { c.User.Id, c.User.Name },
                c.CreatedAt
            });
        }

        [HttpGet("tickets/{id}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound("Ticket not found");

            bool allow = false;
            if (role == "MANAGER") allow = true;
            else if (role == "SUPPORT" && ticket.AssignedTo == userId) allow = true;
            else if (role == "USER" && ticket.CreatedBy == userId) allow = true;

            if (!allow) return StatusCode(403, "Forbidden");

            var comments = await _context.TicketComments
                .Where(c => c.TicketId == id)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            var result = comments.Select(c => new {
                c.Id,
                c.Comment,
                User = new { c.User.Id, c.User.Name },
                c.CreatedAt
            });

            return Ok(result);
        }

        [HttpPatch("comments/{id}")]
        public async Task<IActionResult> EditComment(int id, [FromBody] CommentDTO dto)
        {
            var comment = await _context.TicketComments.FindAsync(id);
            if (comment == null) return NotFound("Comment not found");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            if (role != "MANAGER" && comment.UserId != userId)
                return StatusCode(403, "Forbidden");

            comment.Comment = dto.Comment;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comment updated" });
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.TicketComments.FindAsync(id);
            if (comment == null) return NotFound("Comment not found");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            if (role != "MANAGER" && comment.UserId != userId)
                return StatusCode(403, "Forbidden");

            _context.TicketComments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}
