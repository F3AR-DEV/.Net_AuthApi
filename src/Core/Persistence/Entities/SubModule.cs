using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApi.Core.Persistence.Entities
{
    public class SubModule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Label { get; set; } = string.Empty;

        public int Priority { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(ModuleId))]
        public virtual Module? Module { get; set; }

        public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
    }
}
