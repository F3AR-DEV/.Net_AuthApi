using AuthApi.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Core.Persistence.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasMany(m => m.SubModules)
                  .WithOne(sm => sm.Module)
                  .HasForeignKey(sm => sm.ModuleId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Routes)
                  .WithOne(r => r.Module)
                  .HasForeignKey(r => r.ModuleId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
