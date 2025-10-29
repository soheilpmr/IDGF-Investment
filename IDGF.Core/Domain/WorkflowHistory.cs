using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class WorkflowHistory : Model<int>
    {
        public int WorkflowInstanceId { get; set; }
        public byte StepKey { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string PerformedBy { get; set; } = default!;
        public DateTime PerformedDate { get; set; } = DateTime.UtcNow;
        public string? Comment { get; set; }
    }
}
