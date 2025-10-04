using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDGF.Core.Data.Configurations
{
    public class BondTypesConfiguration : IEntityTypeConfiguration<BondsTypeEntity>
    {
        public void Configure(EntityTypeBuilder<BondsTypeEntity> builder)
        {
            builder.ToTable("opt_BondTypes");
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired().HasColumnName("name");   
            builder.Property(e => e.HasCoupon).IsRequired().HasColumnName("has_coupon");
            builder.Property(e => e.ID).IsRequired().HasColumnName("Id");
        }
    }
}
