using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    [Table("opt_WorkflowInstances")]
    public class WorkflowInstanceEntity : WorkflowInstance
    {
        public WorkflowInstanceEntity()
        {
            
        }

        public WorkflowInstanceEntity(WorkflowInstance workflowInstance)
        {
            WorkflowDefinitionId = workflowInstance.WorkflowDefinitionId;
            ReportId = workflowInstance.ReportId;
            CurrentStepKey = workflowInstance.CurrentStepKey;
            Status = workflowInstance.Status;
            CreatedBy = workflowInstance.CreatedBy;
            CreatedDate = workflowInstance.CreatedDate;
        }

        public ICollection<WorkflowHistoryEntity>? WorkflowHistoryEntities { get; set; }

        [ForeignKey(nameof(WorkflowDefinitionId))]
        public WorkflowDefinitionEntity? WorkflowDefinitionEntity { get; set; }

        [ForeignKey(nameof(ReportId))]
        public ReportEntity? ReportEntity { get; set; }

    }
}
