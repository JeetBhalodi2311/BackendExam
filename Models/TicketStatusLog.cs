using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApiExam.Models
{
    public class TicketStatusLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        public Ticket Ticket { get; set; } = null!;

        [Required]
        public TicketStatus OldStatus { get; set; }

        [Required]
        public TicketStatus NewStatus { get; set; }

        [Required]
        [ForeignKey("ChangedByUser")]
        public int ChangedBy { get; set; }

        public User ChangedByUser { get; set; } = null!;

        public DateTime ChangedAt { get; set; } = DateTime.Now;
    }
}
