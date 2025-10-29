using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    [Table("opt_WorkflowSteps")]
    public class WorkflowStepEntity : WorkflowStep
    {
        [ForeignKey(nameof(WorkflowDefinitionId))]
        public WorkflowDefinitionEntity? WorkflowDefinitionEntity { get; set; }
    }
}
