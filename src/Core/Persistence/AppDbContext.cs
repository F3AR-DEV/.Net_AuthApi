using AuthApi.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Core.Persistence
{
      public class AppDbContext : DbContext
      {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

            public DbSet<User> Users => Set<User>();
            public DbSet<Role> Roles => Set<Role>();
            public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
            public DbSet<Permission> Permissions => Set<Permission>();
            public DbSet<Module> Modules => Set<Module>();
            public DbSet<SubModule> SubModules => Set<SubModule>();
            public DbSet<AuthApi.Core.Persistence.Entities.Route> Routes => Set<AuthApi.Core.Persistence.Entities.Route>();
            public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
            public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  base.OnModelCreating(modelBuilder);
                  modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            }
      }
}
