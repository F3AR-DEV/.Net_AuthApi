using AuthApi.Core.Persistence;
using AuthApi.Core.Persistence.Entities;
using AuthApi.Core.Security;
using AuthApi.Shared.DTOs.Auth;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Modules.Auth.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    }

    public class AuthService(
        AppDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService,
        IConfiguration configuration) : IAuthService
    {
        private readonly AppDbContext _context = context;
        private readonly IPasswordService _passwordService = passwordService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("El usuario ya existe");

            if (request.Password != request.ConfirmPassword)
                throw new Exception("Las contraseñas no coinciden");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password)
            };

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null) user.Roles.Add(defaultRole);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                return null;

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null) return null;

            var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return null;

            var savedRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);
            if (savedRefreshToken == null || savedRefreshToken.IsExpired)
                return null;

            _context.RefreshTokens.Remove(savedRefreshToken);
            await _context.SaveChangesAsync();

            return await GenerateAuthResponse(user);
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var rfDuration = double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"] ?? "7");
            var rfTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(rfDuration),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(rfTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponse(
                accessToken,
                refreshToken,
                rfTokenEntity.ExpiresAt,
                user.Email
            );
        }
    }
}
