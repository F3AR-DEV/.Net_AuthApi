using AuthApi.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Core.Persistence.Configurations
{
    public class SubModuleConfiguration : IEntityTypeConfiguration<SubModule>
    {
        public void Configure(EntityTypeBuilder<SubModule> builder)
        {
            builder.HasMany(sm => sm.Routes)
                  .WithOne(r => r.SubModule)
                  .HasForeignKey(r => r.SubModuleId)
                  .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
