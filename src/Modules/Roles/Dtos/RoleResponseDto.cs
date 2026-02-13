using AuthApi.Modules.Modules.Dtos;

namespace AuthApi.Modules.Roles.Dtos
{
    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<ModuleResponseDto>? Modules { get; set; }
    }
}
