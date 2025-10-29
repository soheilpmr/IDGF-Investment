using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class WorkflowTransition : Model<int>
    {
        public int WorkflowDefinitionId { get; set; }
        public string FromStepKey { get; set; } = default!;
        public string Action { get; set; } = default!;   // trigger
        public string? ToStepKey { get; set; }           // null => end/completed or terminal (can interpret specially)
    }
}
