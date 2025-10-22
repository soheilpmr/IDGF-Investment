using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDGF.Core.Data.Configurations
{
    public class BrokerageConfiguration : IEntityTypeConfiguration<BrokerageEntity>
    {
        public void Configure(EntityTypeBuilder<BrokerageEntity> builder)
        {
            builder.ToTable("opt_Brokers");
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired().HasColumnName("Name");
            //builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
}
