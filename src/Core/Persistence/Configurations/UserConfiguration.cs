using AuthApi.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Core.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(e => e.Email).IsUnique();

            builder.HasMany(u => u.Roles)
                  .WithMany(r => r.Users)
                  .UsingEntity(j => j.ToTable("UserRoles"));
        }
    }
}
