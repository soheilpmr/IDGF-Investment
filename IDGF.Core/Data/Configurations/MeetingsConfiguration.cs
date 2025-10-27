using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Data.Configurations
{
    public class MeetingsConfiguration : IEntityTypeConfiguration<MeetingEntity>
    {
        public void Configure(EntityTypeBuilder<MeetingEntity> builder)
        {
            builder.ToTable("opt_Meetings");
            builder.Property(s => s.ID).HasColumnType("int");
            builder.Property(e => e.MeetingDate).IsRequired();
            builder.Property(e => e.Description).IsRequired(false);
            builder.Property(e => e.CreatedByUserID).IsRequired(false);
            builder.Property(e => e.CreatedAt).IsRequired();
        }
    }
}
