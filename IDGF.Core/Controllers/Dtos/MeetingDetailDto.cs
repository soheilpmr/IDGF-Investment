namespace IDGF.Core.Controllers.Dtos
{
    public class MeetingDetailDto
    {
        public int ID { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? Description { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MeetingFileDto> Files { get; set; } = new List<MeetingFileDto>();
    }

    public class MeetingFileDto
    {
        public int ID { get; set; }
        public string FileType { get; set; }
        public string OriginalFileName { get; set; }
        public int? FileSizeKB { get; set; }
    }
}
