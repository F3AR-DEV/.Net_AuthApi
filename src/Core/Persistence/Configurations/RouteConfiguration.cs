using AuthApi.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Core.Persistence.Configurations
{
    public class RouteConfiguration : IEntityTypeConfiguration<AuthApi.Core.Persistence.Entities.Route>
    {
        public void Configure(EntityTypeBuilder<AuthApi.Core.Persistence.Entities.Route> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Path).IsRequired().HasMaxLength(200);
            builder.Property(r => r.Description).HasMaxLength(500);

            builder.HasOne(r => r.Module)
                .WithMany(m => m.Routes)
                .HasForeignKey(r => r.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.SubModule)
                .WithMany(s => s.Routes)
                .HasForeignKey(r => r.SubModuleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
