using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class WorkflowInstance : Model<int>
    {
        public int WorkflowDefinitionId { get; set; }
      
        // Link to your report (store as int, guid, string per your report table)
        public int ReportId { get; set; }

        public byte CurrentStepKey { get; set; } = default!;
        public string Status { get; set; } = "InProgress"; // InProgress, Completed, Rejected, etc.

        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

  
    }
}
