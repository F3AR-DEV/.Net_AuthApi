using AuthApi.Modules.Users.Dtos;
using AuthApi.Modules.Users.Services;

using AuthApi.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Modules.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Usuarios")]
    public class UsersController(IUsersService usersService) : ControllerBase
    {
        private readonly IUsersService _usersService = usersService;

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetUsers()
        {
            var users = await _usersService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserResponseDto>>.SuccessResponse(users, "Lista de usuarios obtenida"));
        }

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetMe()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(ApiResponse<UserResponseDto>.FailureResponse("Usuario no autenticado"));

            var user = await _usersService.GetByIdAsync(int.Parse(userId));
            if (user == null)
                return NotFound(ApiResponse<UserResponseDto>.FailureResponse("Usuario no encontrado"));

            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user, "Datos del perfil obtenidos"));
        }
    }
}
