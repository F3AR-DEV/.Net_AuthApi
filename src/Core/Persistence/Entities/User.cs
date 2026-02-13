using System.ComponentModel.DataAnnotations;

namespace AuthApi.Core.Persistence.Entities
{

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Dni { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(512)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
