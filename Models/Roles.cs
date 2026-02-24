using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendApiExam.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public class RoleDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
