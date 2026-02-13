using System.ComponentModel.DataAnnotations;

namespace AuthApi.Modules.Roles.Dtos
{
    public class RolePermissionItemDto
    {
        [Required]
        public int RouteId { get; set; }

        [Required]
        public int PermissionId { get; set; }
    }

    public class CreateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<RolePermissionItemDto>? RolePermissions { get; set; }
    }

    public class UpdateRoleDto
    {
        [MaxLength(50)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public List<RolePermissionItemDto>? RolePermissions { get; set; }
    }
}
