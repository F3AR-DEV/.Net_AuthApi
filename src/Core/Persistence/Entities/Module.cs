using System.ComponentModel.DataAnnotations;

namespace AuthApi.Core.Persistence.Entities
{
    public class Module
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Label { get; set; } = string.Empty;

        [Required]
        public string Icon { get; set; } = string.Empty;

        public int Priority { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
        public virtual ICollection<SubModule> SubModules { get; set; } = new List<SubModule>();
    }
}
