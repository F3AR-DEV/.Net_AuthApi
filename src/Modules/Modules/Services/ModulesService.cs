using AuthApi.Core.Persistence;
using AuthApi.Modules.Modules.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Modules.Modules.Services
{
    public interface IModulesService
    {
        Task<List<ModuleResponseDto>> FindAllAsync();
        Task<ModuleResponseDto?> FindOneAsync(int id);
    }

    public class ModulesService : IModulesService
    {
        private readonly AppDbContext _context;

        public ModulesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ModuleResponseDto>> FindAllAsync()
        {
            var modules = await _context.Modules
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Routes)
                .Include(m => m.Routes.Where(r => r.SubModuleId == null))
                .OrderBy(m => m.Priority)
                .ToListAsync();

            return modules.Select(m => MapToDto(m)).ToList();
        }

        public async Task<ModuleResponseDto?> FindOneAsync(int id)
        {
            var module = await _context.Modules
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Routes)
                .Include(m => m.Routes.Where(r => r.SubModuleId == null))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (module == null) return null;

            return MapToDto(module);
        }

        private ModuleResponseDto MapToDto(Core.Persistence.Entities.Module module)
        {
            return new ModuleResponseDto
            {
                Id = module.Id,
                Name = module.Name,
                Label = module.Label,
                Icon = module.Icon,
                Priority = module.Priority,
                Routes = module.Routes
                    .Where(r => r.SubModuleId == null)
                    .Select(r => new RouteResponseDto
                    {
                        Id = r.Id,
                        Path = r.Path,
                        Label = r.Description
                    }).ToList(),
                SubModules = module.SubModules
                    .OrderBy(sm => sm.Priority)
                    .Select(sm => new SubModuleResponseDto
                    {
                        Id = sm.Id,
                        Name = sm.Name,
                        Label = sm.Label,
                        Priority = sm.Priority,
                        Routes = sm.Routes.Select(r => new RouteResponseDto
                        {
                            Id = r.Id,
                            Path = r.Path,
                            Label = r.Description
                        }).ToList()
                    }).ToList()
            };
        }
    }
}
