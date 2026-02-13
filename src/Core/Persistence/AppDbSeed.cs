using AuthApi.Core.Persistence.Entities;
using AuthApi.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Core.Persistence
{
    public static class AppDbSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

            // 1. Limpiar base de datos (Opcional en repro, pero si se usa, debe seguir con Migrate)
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();

            // 1. Crear Permisos base si no existen
            var permissions = new List<Permission>();
            if (!await context.Permissions.AnyAsync())
            {
                permissions = new List<Permission>
                {
                    new() { Name = "Ver", Label = "view" },
                    new() { Name = "Crear", Label = "create" },
                    new() { Name = "Editar", Label = "update" },
                    new() { Name = "Eliminar", Label = "delete" }
                };
                context.Permissions.AddRange(permissions);
                await context.SaveChangesAsync();
            }
            else
            {
                permissions = await context.Permissions.ToListAsync();
            }

            // 2. Crear Módulos y Rutas
            if (!await context.Modules.AnyAsync())
            {
                // Módulo: Seguridad/Configuración
                var configModule = new Module
                {
                    Name = "Configuración",
                    Label = "settings",
                    Icon = "settings_suggest",
                    Priority = 100,
                    Routes = new List<Entities.Route>
                    {
                        new() { Path = "/config/roles", Description = "Gestión de Roles" },
                        new() { Path = "/config/usuarios", Description = "Control de Usuarios" }
                    }
                };

                // Módulo: Gestión Clínica
                var clinicModule = new Module
                {
                    Name = "Gestión Clínica",
                    Label = "clinic",
                    Icon = "medical_services",
                    Priority = 10,
                    Routes = new List<Entities.Route>
                    {
                        new() { Path = "/pacientes", Description = "Listado de Pacientes" },
                        new() { Path = "/citas", Description = "Calendario de Citas" }
                    }
                };

                context.Modules.AddRange(configModule, clinicModule);
                await context.SaveChangesAsync();
            }

            // 3. Crear Roles
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role { Name = "Admin", Description = "Administrador Total del Sistema", IsActive = true };
                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }

            var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole == null)
            {
                userRole = new Role { Name = "User", Description = "Usuario con acceso limitado", IsActive = true };
                context.Roles.Add(userRole);
                await context.SaveChangesAsync();
            }

            // 4. Vincular Rol Admin con TODOS los permisos en TODAS las rutas
            if (!await context.RolePermissions.AnyAsync(p => p.RoleId == adminRole.Id))
            {
                var allRoutes = await context.Routes.ToListAsync();
                foreach (var route in allRoutes)
                {
                    foreach (var perm in permissions)
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            RouteId = route.Id,
                            PermissionId = perm.Id
                        });
                    }
                }
                await context.SaveChangesAsync();
            }

            // 5. Vincular Rol User con Módulo Gestión Clínica
            if (!await context.RolePermissions.AnyAsync(p => p.RoleId == userRole.Id))
            {
                var clinicModule = await context.Modules
                    .Include(m => m.Routes)
                    .FirstOrDefaultAsync(m => m.Name == "Gestión Clínica");

                if (clinicModule != null && clinicModule.Routes != null)
                {
                    foreach (var route in clinicModule.Routes)
                    {
                        foreach (var perm in permissions)
                        {
                            context.RolePermissions.Add(new RolePermission
                            {
                                RoleId = userRole.Id,
                                RouteId = route.Id,
                                PermissionId = perm.Id
                            });
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }

            // 5. Crear Usuarios
            if (!await context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    Email = "admin@authapi.com",
                    PasswordHash = passwordService.HashPassword("Admin123!"),
                    FirstName = "Admin",
                    LastName = "System",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                adminUser.Roles.Add(adminRole);

                var regularUser = new User
                {
                    Email = "user@authapi.com",
                    PasswordHash = passwordService.HashPassword("User123!"),
                    FirstName = "Regular",
                    LastName = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                regularUser.Roles.Add(userRole);

                context.Users.AddRange(adminUser, regularUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
