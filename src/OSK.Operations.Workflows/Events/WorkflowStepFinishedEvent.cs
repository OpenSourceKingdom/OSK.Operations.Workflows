using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

public class WorkflowStepFinishedEvent(WorkflowStep workflowStep) : WorkflowEvent
{
    public WorkflowStep WorkflowStep { get; } = workflowStep;
}
