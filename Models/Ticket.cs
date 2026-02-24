using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackendApiExam.Models
{
    public enum TicketStatus
    {
        OPEN,
        IN_PROGRESS,
        RESOLVED,
        CLOSED
    }

    public enum TicketPriority
    {
        LOW,
        MEDIUM,
        HIGH
    }

    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [MinLength(5)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.OPEN;

        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.MEDIUM;

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }

        public User CreatedByUser { get; set; } = null!;

        [ForeignKey("AssignedToUser")]
        public int? AssignedTo { get; set; }

        public User? AssignedToUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();

        [JsonIgnore]
        public ICollection<TicketStatusLog> StatusLogs { get; set; } = new List<TicketStatusLog>();
    }
}
