using AuthApi.Core.Persistence;
using AuthApi.Core.Persistence.Entities;
using AuthApi.Modules.Roles.Dtos;
using AuthApi.Modules.Modules.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Modules.Roles.Services
{
    public interface IRolesService
    {
        Task<List<RoleResponseDto>> FindAllAsync();
        Task<RoleResponseDto> FindOneAsync(int id);
        Task<RoleResponseDto> CreateAsync(CreateRoleDto dto);
        Task<RoleResponseDto> UpdateAsync(int id, UpdateRoleDto dto);
    }

    public class RolesService : IRolesService
    {
        private readonly AppDbContext _context;

        public RolesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleResponseDto>> FindAllAsync()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Route)
                        .ThenInclude(rt => rt!.Module)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Route)
                        .ThenInclude(rt => rt!.SubModule)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            return roles.Select(MapToResponseDto).ToList();
        }

        public async Task<RoleResponseDto> FindOneAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Route)
                        .ThenInclude(rt => rt!.Module)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Route)
                        .ThenInclude(rt => rt!.SubModule)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) throw new KeyNotFoundException($"Rol con id {id} no encontrado");

            return MapToResponseDto(role);
        }

        public async Task<RoleResponseDto> CreateAsync(CreateRoleDto dto)
        {
            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            if (dto.RolePermissions != null)
            {
                foreach (var rp in dto.RolePermissions)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        RouteId = rp.RouteId,
                        PermissionId = rp.PermissionId
                    });
                }
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return await FindOneAsync(role.Id);
        }

        public async Task<RoleResponseDto> UpdateAsync(int id, UpdateRoleDto dto)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) throw new KeyNotFoundException($"Rol con id {id} no encontrado");

            if (dto.Name != null) role.Name = dto.Name;
            if (dto.Description != null) role.Description = dto.Description;
            if (dto.IsActive.HasValue) role.IsActive = dto.IsActive.Value;

            if (dto.RolePermissions != null)
            {
                _context.RolePermissions.RemoveRange(role.RolePermissions);
                role.RolePermissions.Clear();

                foreach (var rp in dto.RolePermissions)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        RoleId = id,
                        RouteId = rp.RouteId,
                        PermissionId = rp.PermissionId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return await FindOneAsync(id);
        }

        private RoleResponseDto MapToResponseDto(Role role)
        {
            var response = new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                Modules = new List<ModuleResponseDto>()
            };

            var modulesDict = new Dictionary<int, ModuleResponseDto>();

            foreach (var rp in role.RolePermissions)
            {
                var route = rp.Route;
                if (route == null) continue;

                var module = route.Module;
                if (module == null) continue;

                if (!modulesDict.TryGetValue(module.Id, out var moduleDto))
                {
                    moduleDto = new ModuleResponseDto
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Label = module.Label,
                        Icon = module.Icon,
                        Priority = module.Priority,
                        SubModules = new List<SubModuleResponseDto>(),
                        Routes = new List<RouteResponseDto>()
                    };
                    modulesDict.Add(module.Id, moduleDto);
                }

                if (route.SubModuleId.HasValue)
                {
                    var subModule = route.SubModule;
                    if (moduleDto.SubModules == null) moduleDto.SubModules = new List<SubModuleResponseDto>();

                    var subModuleDto = moduleDto.SubModules.FirstOrDefault(s => s.Id == subModule!.Id);
                    if (subModuleDto == null)
                    {
                        subModuleDto = new SubModuleResponseDto
                        {
                            Id = subModule!.Id,
                            Name = subModule.Name,
                            Label = subModule.Label,
                            Priority = subModule.Priority,
                            Routes = new List<RouteResponseDto>()
                        };
                        moduleDto.SubModules.Add(subModuleDto);
                    }
                    AddRouteToContainer(subModuleDto.Routes, rp);
                }
                else
                {
                    if (moduleDto.Routes == null) moduleDto.Routes = new List<RouteResponseDto>();
                    AddRouteToContainer(moduleDto.Routes, rp);
                }
            }

            response.Modules = modulesDict.Values.OrderBy(m => m.Priority).ToList();

            // Limpieza y ordenamiento
            foreach (var m in response.Modules)
            {
                if (m.SubModules != null && m.SubModules.Any())
                {
                    m.SubModules = m.SubModules.OrderBy(s => s.Priority).ToList();
                }
                else
                {
                    m.SubModules = null;
                }

                if (m.Routes != null && !m.Routes.Any())
                {
                    m.Routes = null;
                }
            }

            return response;
        }

        private void AddRouteToContainer(List<RouteResponseDto> container, RolePermission rp)
        {
            var routeDto = container.FirstOrDefault(r => r.Id == rp.RouteId);
            if (routeDto == null)
            {
                routeDto = new RouteResponseDto
                {
                    Id = rp.RouteId,
                    Path = rp.Route?.Path ?? string.Empty,
                    Label = rp.Route?.Description,
                    Permissions = new PermissionMapDto()
                };
                container.Add(routeDto);
            }

            if (routeDto.Permissions == null) routeDto.Permissions = new PermissionMapDto();

            var permName = rp.Permission?.Name.ToLower() ?? "";
            switch (permName)
            {
                case "ver":
                case "view":
                    routeDto.Permissions.View = true;
                    break;
                case "crear":
                case "create":
                    routeDto.Permissions.Create = true;
                    break;
                case "editar":
                case "update":
                case "edit":
                    routeDto.Permissions.Update = true;
                    break;
                case "eliminar":
                case "delete":
                    routeDto.Permissions.Delete = true;
                    break;
            }
        }
    }
}
