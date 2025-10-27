using BackEndInfrsastructure.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IDGF.Core.Domain
{
    public class MeetingFile : Model<int>
    {
        public int MeetingID { get; set; } 
        public string FileType { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFilePath { get; set; } = string.Empty;
        public int? FileSizeKB { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}
