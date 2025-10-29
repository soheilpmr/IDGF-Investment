using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class WorkflowDefinition : Model<int>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
