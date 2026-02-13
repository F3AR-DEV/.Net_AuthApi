using Microsoft.Extensions.DependencyInjection;

namespace AuthApi.Shared.Security
{
    public static class AuthorizationConfig
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });
            return services;
        }
    }
}
