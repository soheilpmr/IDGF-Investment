using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    public class MeetingFileEntity : MeetingFile
    {

        public MeetingFileEntity()
        {
        }

        public MeetingFileEntity(MeetingFile file)
        {
            this.MeetingID = file.MeetingID;
            this.FileType = file.FileType;
            this.OriginalFileName = file.OriginalFileName;
            this.StoredFilePath = file.StoredFilePath;
            this.FileSizeKB = file.FileSizeKB;
            this.UploadedAt = file.UploadedAt;
        }
        [ForeignKey(nameof(MeetingID))]
        public virtual MeetingEntity? Meeting { get; set; }
    }
}
