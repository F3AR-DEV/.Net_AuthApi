using AuthApi.Core.Security;
using AuthApi.Modules.Auth.Services;
using AuthApi.Modules.Modules.Services;
using AuthApi.Modules.Roles.Services;
using AuthApi.Modules.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApi.Shared.DependencyInjection
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IModulesService, ModulesService>();
            services.AddScoped<IUsersService, UsersService>();

            return services;
        }
    }
}
