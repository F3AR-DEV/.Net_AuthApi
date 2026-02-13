using AuthApi.Modules.Modules.Dtos;
using AuthApi.Modules.Modules.Services;

using AuthApi.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Modules.Modules.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("Módulos")]
    public class ModulesController(IModulesService modulesService) : ControllerBase
    {
        private readonly IModulesService _modulesService = modulesService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ModuleResponseDto>>>> GetAll()
        {
            var result = await _modulesService.FindAllAsync();
            return Ok(ApiResponse<List<ModuleResponseDto>>.SuccessResponse(result, "Módulos obtenidos con éxito"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ModuleResponseDto>>> GetById(int id)
        {
            var module = await _modulesService.FindOneAsync(id);
            if (module == null)
                return NotFound(ApiResponse<ModuleResponseDto>.FailureResponse($"Módulo con id {id} no encontrado"));

            return Ok(ApiResponse<ModuleResponseDto>.SuccessResponse(module, "Detalle del módulo obtenido"));
        }
    }
}
