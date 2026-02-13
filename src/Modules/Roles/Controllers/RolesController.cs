using Asp.Versioning;
using AuthApi.Modules.Roles.Dtos;
using AuthApi.Modules.Roles.Services;

using AuthApi.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AuthApi.Modules.Roles.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Roles")]
    [Authorize]
    public class RolesController(IRolesService rolesService) : ControllerBase
    {
        private readonly IRolesService _rolesService = rolesService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoleResponseDto>>>> GetAll()
        {
            var roles = await _rolesService.FindAllAsync();
            return Ok(ApiResponse<List<RoleResponseDto>>.SuccessResponse(roles, "Roles obtenidos con éxito"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleResponseDto>>> GetById(int id)
        {
            try
            {
                var role = await _rolesService.FindOneAsync(id);
                return Ok(ApiResponse<RoleResponseDto>.SuccessResponse(role, "Detalle del rol obtenido"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<RoleResponseDto>.FailureResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleResponseDto>>> Create([FromBody] CreateRoleDto dto)
        {
            var role = await _rolesService.CreateAsync(dto);
            return Ok(ApiResponse<RoleResponseDto>.SuccessResponse(role, "Rol creado exitosamente"));
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponse<RoleResponseDto>>> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            try
            {
                var role = await _rolesService.UpdateAsync(id, dto);
                return Ok(ApiResponse<RoleResponseDto>.SuccessResponse(role, "Rol actualizado con éxito"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<RoleResponseDto>.FailureResponse(ex.Message));
            }
        }
    }
}
