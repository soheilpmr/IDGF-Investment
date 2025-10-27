using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Data.Configurations
{
    public class MeetingFilesConfiguration : IEntityTypeConfiguration<MeetingFileEntity>
    {
        public void Configure(EntityTypeBuilder<MeetingFileEntity> builder)
        {
            builder.ToTable("opt_MeetingFiles");
            builder.Property(s => s.ID).HasColumnType("int");
            builder.Property(e => e.MeetingID).HasColumnType("int").IsRequired();
            builder.Property(e => e.FileType).HasMaxLength(100).IsRequired();
            builder.Property(e => e.OriginalFileName).HasMaxLength(255).IsRequired();
            builder.Property(e => e.StoredFilePath).HasMaxLength(500).IsRequired();
            builder.Property(e => e.FileSizeKB).IsRequired(false);
            builder.Property(e => e.UploadedAt).IsRequired();

            builder.HasOne(file => file.Meeting)
                .WithMany(meeting => meeting.Files)
                .HasForeignKey(file => file.MeetingID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
