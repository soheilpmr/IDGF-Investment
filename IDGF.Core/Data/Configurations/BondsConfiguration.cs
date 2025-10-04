using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDGF.Core.Data.Configurations
{
    public class BondsConfiguration : IEntityTypeConfiguration<BondsEntity>
    {
        public void Configure(EntityTypeBuilder<BondsEntity> builder)
        {
            builder.ToTable("opt_Bonds");
            builder.Property(s => s.ID).HasColumnType("decimal(18,0)");
            builder.Property(e => e.TypeID).IsRequired();
            builder.Property(e => e.Symbol).IsRequired();   
            builder.Property(e => e.IssueDate);
            builder.Property(e => e.MaturityDate).IsRequired();
            builder.Property(e => e.FaceValue).IsRequired().HasColumnType("decimal(18,0)");
            builder.Property(e => e.CouponRatePercen).HasColumnType("decimal(5,2)");

            builder.HasOne(b => b.BondTypesEntity)
              .WithMany(t => t.BondsEntities)
              .HasForeignKey(b => b.TypeID)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
