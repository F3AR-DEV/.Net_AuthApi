using AuthApi.Core.Persistence;
using AuthApi.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Modules.Maintenance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Mantenimiento")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public class MaintenanceController(AppDbContext context, IServiceProvider serviceProvider) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [HttpGet("seed-status")]
        public async Task<ActionResult<ApiResponse<object>>> GetSeedStatus()
        {
            var status = new
            {
                HasPermissions = await _context.Permissions.AnyAsync(),
                HasModules = await _context.Modules.AnyAsync(),
                HasRoles = await _context.Roles.AnyAsync(),
                HasUsers = await _context.Users.AnyAsync(),
                Timestamp = DateTime.UtcNow
            };

            var isSeeded = status.HasPermissions && status.HasModules && status.HasRoles && status.HasUsers;

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                IsSeeded = isSeeded,
                Details = status
            }, isSeeded ? "La base de datos tiene datos iniciales" : "La semilla no se ha completado"));
        }

        [HttpPost("initialize")]
        public async Task<ActionResult<ApiResponse<string>>> InitializeDatabase()
        {
            try
            {
                // Solo ejecuta migraciones y seed sin borrar
                await AppDbSeed.SeedAsync(_serviceProvider);
                return Ok(ApiResponse<string>.SuccessResponse("Base de datos inicializada (migraciones y seed aplicados)"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailureResponse($"Error al inicializar: {ex.Message}"));
            }
        }

        [HttpPost("reset")]
        public async Task<ActionResult<ApiResponse<string>>> ResetDatabase()
        {
            try
            {
                // Borra todo y vuelve a empezar
                await _context.Database.EnsureDeletedAsync();
                await AppDbSeed.SeedAsync(_serviceProvider);
                return Ok(ApiResponse<string>.SuccessResponse("Base de datos reseteada completamente (Borrado + Migraciones + Seed)"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailureResponse($"Error al resetear: {ex.Message}"));
            }
        }

        [HttpPost("clear")]
        public async Task<ActionResult<ApiResponse<string>>> ClearDatabase()
        {
            try
            {
                // Borra la base de datos y solo aplica migraciones (queda vacía)
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.MigrateAsync();
                return Ok(ApiResponse<string>.SuccessResponse("Base de datos limpiada (Esquema recreado, sin datos)"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailureResponse($"Error al limpiar: {ex.Message}"));
            }
        }
    }
}
