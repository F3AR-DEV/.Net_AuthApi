using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Shared.Utilities
{
    public static class SecurityUtils
    {
        public static SymmetricSecurityKey CreateSigningKey(string secret)
        {
            using var sha = SHA256.Create();
            var secretBytes = Encoding.UTF8.GetBytes(secret ?? string.Empty);
            var keyBytes = sha.ComputeHash(secretBytes);
            return new SymmetricSecurityKey(keyBytes);
        }
    }
}
