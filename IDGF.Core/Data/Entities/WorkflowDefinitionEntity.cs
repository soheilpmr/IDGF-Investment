using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    [Table("opt_WorkflowDefinitions")]
    public class WorkflowDefinitionEntity : WorkflowDefinition
    {
        public ICollection<WorkflowStepEntity>? WorkflowStepEntities { get; set; }
        public ICollection<WorkflowTransitionEntity>? WorkflowTransitionEntities { get; set; }
    }
}
