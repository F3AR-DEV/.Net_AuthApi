namespace AuthApi.Modules.Modules.Dtos
{
    public class PermissionMapDto
    {
        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }

    public class RouteResponseDto
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public string? Label { get; set; }
        public PermissionMapDto? Permissions { get; set; }
    }

    public class SubModuleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int Priority { get; set; }
        public List<RouteResponseDto> Routes { get; set; } = new();
    }

    public class ModuleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Priority { get; set; }
        public List<RouteResponseDto>? Routes { get; set; }
        public List<SubModuleResponseDto>? SubModules { get; set; }
    }
}
