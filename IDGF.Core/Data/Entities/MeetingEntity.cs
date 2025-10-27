using IDGF.Core.Domain;

namespace IDGF.Core.Data.Entities
{
    public class MeetingEntity : Meeting
    {
        public MeetingEntity()
        {
        }

        public MeetingEntity(Meeting meeting)
        {
            this.MeetingDate = meeting.MeetingDate;
            this.Description = meeting.Description;
            this.CreatedByUserID = meeting.CreatedByUserID;
            this.CreatedAt = meeting.CreatedAt;
        }
        public virtual ICollection<MeetingFileEntity> Files { get; set; } = new List<MeetingFileEntity>();
    }
}
