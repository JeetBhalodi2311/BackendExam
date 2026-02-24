using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackendApiExam.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public Role Role { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();

        [JsonIgnore]
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

        [JsonIgnore]
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }
}
