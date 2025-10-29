using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    [Table("opt_WorkflowHistories")]
    public class WorkflowHistoryEntity : WorkflowHistory
    {
        [ForeignKey(nameof(WorkflowInstanceId))]
        public WorkflowInstanceEntity? WorkflowInstanceEntity { get; set; }
    }
}
