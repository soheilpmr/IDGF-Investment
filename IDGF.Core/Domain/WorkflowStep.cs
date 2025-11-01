using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class WorkflowStep : Model<int>
    {
        public int WorkflowDefinitionId { get; set; }
        // Using StepKey as the state string in the state machine
        public byte StepKey { get; set; } = default!;    // e.g. "Draft", "Review", "Approved"
        public string? DisplayName { get; set; }
        public int StepOrder { get; set; }

        // Role or comma-separated roles allowed to perform actions here
        public string? AssignedRole { get; set; } // e.g. "Reporter", "Reviewer"
        public string? PossibleActions { get; set; } // e.g. "Submit,Approve,Reject,Return,Finish"
    }
}
