using System.ComponentModel.DataAnnotations;

namespace BackendApiExam.Models
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class CreateUserDTO
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }
    }

    public class CreateTicketDTO
    {
        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        public string Description { get; set; } = string.Empty;

        public TicketPriority Priority { get; set; } = TicketPriority.MEDIUM;
    }

    public class AssignDTO
    {
        [Required]
        public int UserId { get; set; }
    }

    public class UpdateStatusDTO
    {
        [Required]
        public TicketStatus Status { get; set; }
    }

    public class CommentDTO
    {
        [Required]
        public string Comment { get; set; } = string.Empty;
    }
}
