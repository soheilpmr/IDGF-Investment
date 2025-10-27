using BackEndInfrsastructure.Domain;
using System.ComponentModel.DataAnnotations;

namespace IDGF.Core.Domain
{
    public class Meeting : Model<int>
    {
        public DateTime MeetingDate { get; set; }
        public string? Description { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
