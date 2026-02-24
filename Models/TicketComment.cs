using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApiExam.Models
{
    public class TicketComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        public Ticket Ticket { get; set; } = null!;

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
