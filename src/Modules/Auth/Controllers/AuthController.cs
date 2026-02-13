using AuthApi.Modules.Auth.Services;

using AuthApi.Shared.DTOs.Auth;
using AuthApi.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Modules.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Autenticación")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result!, "Usuario registrado exitosamente"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(ApiResponse<AuthResponse>.FailureResponse("Credenciales inválidas"));

            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Login exitoso"));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result == null)
                return Unauthorized(ApiResponse<AuthResponse>.FailureResponse("Token de refresco inválido o expirado"));

            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Token renovado exitosamente"));
        }
    }
}
