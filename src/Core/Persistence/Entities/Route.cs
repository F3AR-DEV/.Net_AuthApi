using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApi.Core.Persistence.Entities
{
    public class Route
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        public int? SubModuleId { get; set; }

        [Required]
        public string Path { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(ModuleId))]
        public virtual Module? Module { get; set; }

        [ForeignKey(nameof(SubModuleId))]
        public virtual SubModule? SubModule { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
