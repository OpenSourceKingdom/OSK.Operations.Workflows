using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

public class WorkflowStepOperationFinishedEvent(WorkflowStep workflowStep, ITaskOperation operation)
    : TaskOperationFinishedEvent(operation)
{
    public WorkflowStep WorkflowStep { get; } = workflowStep;
}
