using System.ComponentModel.DataAnnotations;

namespace AuthApi.Shared.DTOs.Auth
{
    public record LoginRequest(
        [Required][EmailAddress] string Email,
        [Required] string Password
    );

    public record RegisterRequest(
        [Required][EmailAddress] string Email,
        [Required][MinLength(6)] string Password,
        [Required] string ConfirmPassword
    );

    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime IsExpiredAt,
        string Email
    );

    public record RefreshTokenRequest(
        [Required] string AccessToken,
        [Required] string RefreshToken
    );
}
