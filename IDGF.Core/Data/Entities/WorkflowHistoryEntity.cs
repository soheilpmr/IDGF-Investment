using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    [Table("opt_WorkflowHistories")]
    public class WorkflowHistoryEntity : WorkflowHistory
    {
        public WorkflowHistoryEntity()
        {
            
        }

        public WorkflowHistoryEntity(WorkflowHistory workflowHistory)
        {
            WorkflowInstanceId = workflowHistory.WorkflowInstanceId;
            StepKey = workflowHistory.StepKey;
            Action = workflowHistory.Action;
            PerformedBy = workflowHistory.PerformedBy;
            PerformedDate = workflowHistory.PerformedDate;
            Comment = workflowHistory.Comment;
        }

        [ForeignKey(nameof(WorkflowInstanceId))]
        public WorkflowInstanceEntity? WorkflowInstanceEntity { get; set; }
    }
}
