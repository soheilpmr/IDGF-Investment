using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDGF.Core.Data.Configurations
{
    public class TransactionsConfiguration : IEntityTypeConfiguration<TransactionsEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionsEntity> builder)
        {
            builder.ToTable("opt_transactions");
            builder.Property(s => s.ID).HasColumnType("decimal(18,0)");
            builder.Property(e => e.TransactionDate).IsRequired();
            builder.Property(e => e.TransactionType).IsRequired();
            builder.Property(e => e.Quantity).HasColumnType("decimal(18,0)");
            builder.Property(e => e.PricePerUnit).HasColumnType("decimal(18,0)");
            builder.Property(e => e.Commission).IsRequired().HasColumnType("decimal(18,0)");
            builder.Property(e => e.YtmAtTransaction).IsRequired().HasColumnType("decimal(10,0)");
            builder.Property(e => e.Status).HasColumnType("smallint").IsRequired();

            builder.HasOne(b => b.BondsEntity)
              .WithMany(t => t.TransactionsEntities)
              .HasForeignKey(b => b.BondId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.HasOne(b => b.BrokerageEntity)
            .WithMany(t => t.TransactionsEntities)
            .HasForeignKey(b => b.BrokerId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}
