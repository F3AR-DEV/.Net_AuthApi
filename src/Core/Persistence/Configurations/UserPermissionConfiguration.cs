using AuthApi.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Core.Persistence.Configurations
{
    public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.HasOne(up => up.User)
                  .WithMany(u => u.UserPermissions)
                  .HasForeignKey(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.Route)
                  .WithMany(r => r.UserPermissions)
                  .HasForeignKey(up => up.RouteId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.Permission)
                  .WithMany(p => p.UserPermissions)
                  .HasForeignKey(up => up.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
