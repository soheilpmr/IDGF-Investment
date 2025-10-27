using System.ComponentModel.DataAnnotations;

namespace IDGF.Core.Controllers.Dtos
{
    public class CreateMeetingRequestDto
    {
        [Required]
        public DateTime MeetingDate { get; set; }
        public string? Description { get; set; }

        // گزارش ۳۰ به ۷۰
        public IFormFile? Report30To70 { get; set; }

        // گزارش بازدید دوره ای
        public IFormFile? PurchasedBondsReport { get; set; }

        // صورتجلسه
        public IFormFile? Minutes { get; set; }

        // گزارش اوراق سررسید شده
        public IFormFile? MaturedBondsReport { get; set; }

        // گزارش جریان وجوه نقد
        public IFormFile? CashFlowReport { get; set; }
    }
}
