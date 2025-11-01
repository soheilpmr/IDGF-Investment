using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Stateless;

namespace IDGF.Core.Services.WorkFlow
{
    public class WorkFlowService : IWorkflowService
    {
        private readonly WorkFlowDbContext _context;

        public WorkFlowService(WorkFlowDbContext workFlowDbContext)
        {
            _context = workFlowDbContext;    
        }

        public async Task<WorkflowInstance?> GetInstanceAsync(int instanceId)
        {
            return await _context.WorkflowInstances
                   .Include(i => i.WorkflowHistoryEntities)
                   .Include(i => i.WorkflowDefinitionEntity)
                       .ThenInclude(w => w.WorkflowStepEntities)
                   .FirstOrDefaultAsync(i => i.ID == instanceId);
        }

        public async Task<bool> PerformActionAsync(int instanceId, string action, string performedBy, string? comment = null)
        {
            // Load instance and the definition
            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinitionEntity)
                    .ThenInclude(w => w.WorkflowStepEntities)
                .Include(i => i.WorkflowDefinitionEntity)
                    .ThenInclude(w => w.WorkflowTransitionEntities)
                .FirstOrDefaultAsync(i => i.ID == instanceId);

            if (instance == null) throw new InvalidOperationException("Workflow instance not found.");
            if (instance.Status != WorkflowInstanceStatus.InProgress.ToString()) return false;

            var wf = instance.WorkflowDefinitionEntity!;
            var currentStepKey = instance.CurrentStepKey;

            // Verify the action is allowed in current step
            var currentStep = wf.WorkflowStepEntities.FirstOrDefault(s => s.StepKey == currentStepKey)
                ?? throw new InvalidOperationException("Current step not found in workflow definition.");

            // check role (if AssignedRole set)
            if (!string.IsNullOrWhiteSpace(currentStep.AssignedRole))
            {
                // support comma-separated roles
                var roles = currentStep.AssignedRole.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var allowed = false;
                foreach (var r in roles)
                {
                    //if (await _identity.UserHasRoleAsync(performedBy, r))
                    //{
                    //    allowed = true;
                    //    break;
                    //}
                }

                if (!allowed)
                    throw new UnauthorizedAccessException("User is not permitted to perform actions in current step.");
            }

            // Build state machine dynamically using Stateless
            var stateMachine = new StateMachine<byte, string>(currentStepKey);

            // Configure states and transitions
            // We create configurations for every step seen in definition
            var steps = wf.WorkflowStepEntities.ToList();
            foreach (var step in steps)
            {
                stateMachine.Configure(step.StepKey);

                // find transitions that start from this step
                var outgoing = wf.WorkflowTransitionEntities.Where(t => t.FromStepKey == step.StepKey).ToList();
                foreach (var tr in outgoing)
                {
                    // If ToStepKey is null or empty -> terminal. We'll represent terminal by special state "__END__"
                    byte to = tr.ToStepKey is null ? (byte)4 : tr.ToStepKey.Value;
                    stateMachine.Configure(step.StepKey)
                                .Permit(tr.Action, to);
                }
            }

            // Optionally configure the special end state so firing into it is allowed.
            stateMachine.Configure(4);

            // Validate that the requested action is permitted from the current state
            var allowedTriggers = stateMachine.PermittedTriggers.ToList();
            if (!allowedTriggers.Contains(action))
            {
                throw new InvalidOperationException($"Action '{action}' is not valid from state '{currentStepKey}'. Allowed: {string.Join(',', allowedTriggers)}");
            }

            // Fire the action
            stateMachine.Fire(action);

            // Determine the resulting state
            var newState = stateMachine.State;

            // Persist change
            if (newState == 4)
            {
                instance.Status = action.Equals(WorkflowStepPossibleActions.Finish.ToString(), StringComparison.OrdinalIgnoreCase)
                    ? WorkflowInstanceStatus.Completed.ToString() : WorkflowInstanceStatus.InProgress.ToString();
                instance.CurrentStepKey = newState;
            }
            if (newState == 0)
            {
                instance.Status = action.Equals(WorkflowStepPossibleActions.Reject.ToString(), StringComparison.OrdinalIgnoreCase)
                    ? WorkflowInstanceStatus.Rejected.ToString() : WorkflowInstanceStatus.InProgress.ToString();
                instance.CurrentStepKey = newState;
            }
            else
            {
                // ensure the new state exists in Steps; if not, it's an error in definition
                var nextStep = wf.WorkflowStepEntities.FirstOrDefault(s => s.StepKey == newState);
                if (nextStep == null)
                    throw new InvalidOperationException("Transition leads to unknown step: " + newState);

                instance.CurrentStepKey = nextStep.StepKey;
            }

            // Append history
            var hist = new WorkflowHistory
            {
                WorkflowInstanceId = instance.ID,
                StepKey = currentStepKey,
                Action = action,
                PerformedBy = performedBy,
                PerformedDate = DateTime.UtcNow,
                Comment = comment
            };

            WorkflowHistoryEntity workflowHistoryEntity = new WorkflowHistoryEntity(hist);

            _context.WorkflowHistories.Add(workflowHistoryEntity);

            // Update DB
            _context.WorkflowInstances.Update(instance);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<WorkflowInstance> StartWorkflowAsync(int reportId, string workflowName, string startedBy)
        {
            var wf = await _context.WorkflowDefinitions
                .Include(w => w.WorkflowStepEntities)
                .Include(w => w.WorkflowTransitionEntities)
                .FirstOrDefaultAsync(w => w.Name == workflowName && w.IsActive);

            if (wf == null) throw new InvalidOperationException($"Workflow '{workflowName}' not found or inactive.");

            // Choose the initial step as the lowest StepOrder
            var initialStep = wf.WorkflowStepEntities
                .Where(ss => ss.StepKey == (int)WorkflowStepKey.StartByExpert)
                .FirstOrDefault();

            if (initialStep == null) throw new InvalidOperationException("Workflow has no steps.");

            var instance = new WorkflowInstance
            {
                WorkflowDefinitionId = wf.ID,
                ReportId = reportId,
                CurrentStepKey = initialStep.StepKey,
                Status = WorkflowInstanceStatus.InProgress.ToString(),
                CreatedBy = startedBy,
                CreatedDate = DateTime.Now
            };
            var entity = new WorkflowInstanceEntity(instance);

            _context.WorkflowInstances.Add(entity);
            await _context.SaveChangesAsync();
            return instance;
        }
    }
}
