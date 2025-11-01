using IDGF.Core.Domain;

namespace IDGF.Core.Services.WorkFlow
{
    public interface IWorkflowService
    {
        Task<WorkflowInstance> StartWorkflowAsync(int reportId, string workflowName, string startedBy);
        Task<bool> PerformActionAsync(int instanceId, string action, string performedBy, string? comment = null);
        Task<WorkflowInstance?> GetInstanceAsync(int instanceId);
    }
}
