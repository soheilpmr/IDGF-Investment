using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDGF.Core.Data.Configurations
{
    public class MandehConfiguration : IEntityTypeConfiguration<MandehTransactionsEntity>
    {
        public void Configure(EntityTypeBuilder<MandehTransactionsEntity> builder)
        {
            builder.ToTable("MandehTransactions");
        }

    }
}
