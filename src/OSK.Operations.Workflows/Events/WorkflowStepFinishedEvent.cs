using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

/// <summary>
/// A workflow event that is raised when a workflow step has finished executing.
/// </summary>
/// <param name="workflowStep"></param>
public class WorkflowStepFinishedEvent(WorkflowStep workflowStep) : WorkflowEvent
{
    /// <summary>
    /// The step that finished executing.
    /// </summary>
    public WorkflowStep WorkflowStep { get; } = workflowStep;
}
