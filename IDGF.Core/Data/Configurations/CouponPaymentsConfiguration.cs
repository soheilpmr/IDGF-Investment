using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Data.Configurations
{
    public class CouponPaymentsConfiguration : IEntityTypeConfiguration<CouponPaymentsEntity>
    {

        public void Configure(EntityTypeBuilder<CouponPaymentsEntity> builder)
        {
            builder.ToTable("opt_CouponPayments");
            builder.Property(s => s.ID)
            .HasColumnName("Id")
            .HasColumnType("decimal(18,0)")
            .ValueGeneratedOnAdd();

            builder.Property(e => e.AmountPerUnit)
                .HasColumnName("AmountPerUnit")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.PaymentDate)
                .HasColumnName("PaymentDate") 
                .IsRequired();

            builder.Property(e => e.BondId)
                .HasColumnName("BondId") 
                .HasColumnType("decimal(18,0)")
                .IsRequired();

            builder.HasOne(cp => cp.BondsEntity)
                .WithMany(b => b.CouponPayments)
                .HasForeignKey(cp => cp.BondId) 
                .OnDelete(DeleteBehavior.ClientNoAction);

        }
    }
}
