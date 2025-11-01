using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class WorkflowTransition : Model<int>
    {
        public int WorkflowDefinitionId { get; set; }
        public byte FromStepKey { get; set; } = default!;
        public string Action { get; set; } = default!;   // trigger
        public byte? ToStepKey { get; set; }           // null => end/completed or terminal (can interpret specially)
    }
}
