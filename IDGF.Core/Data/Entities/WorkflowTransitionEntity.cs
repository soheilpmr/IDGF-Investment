using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    [Table("opt_WorkflowTransitions")]
    public class WorkflowTransitionEntity : WorkflowTransition
    {
        [ForeignKey(nameof(WorkflowDefinitionId))]
        public WorkflowDefinitionEntity? WorkflowDefinitionEntity { get; set; }
    }
}
