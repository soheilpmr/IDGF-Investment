namespace IDGF.Core.Controllers.Dtos
{
    public class MeetingResponseDto
    {
        public int ID { get; set; }
        public string MeetingDate { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool HasMinutes { get; set; }
        public bool HasMaturedBondsReport { get; set; }
        public bool HasCashFlowReport { get; set; }
        public bool HasPurchasedBondsReport { get; set; }
        public bool HasReport30To70 { get; set; }
    }

    public class FileLinkDto
    {
        public int ID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }
}
