using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class Report : Model<int>
    {
        public string FileType { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFilePath { get; set; } = string.Empty;
        public int? FileSizeKB { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public int UploadedBy { get; set; } 
    }
}
