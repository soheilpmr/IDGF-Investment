using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    [Table("opt_Reports")]
    public class ReportEntity : Report
    {
        public ReportEntity()
        {
            
        }
        public ReportEntity(Report report)
        {
                FileType = report.FileType; 
                OriginalFileName = report.OriginalFileName;
                StoredFilePath = report.StoredFilePath;
                FileSizeKB = report.FileSizeKB;
                UploadedAt = report.UploadedAt;
                UploadedBy = report.UploadedBy;
        }
    }
}
