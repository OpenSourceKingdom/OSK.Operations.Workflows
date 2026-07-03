using OSK.Operations.Workflows.Models;

namespace OSK.Operations.Workflows.Events;

/// <summary>
/// A workflow operation event that is raised when a workflow step operation has finished executing.
/// </summary>
/// <param name="workflowStep">The step the operation is associated with</param>
/// <param name="operation">The operation that finished executing</param>
public class WorkflowStepOperationFinishedEvent(WorkflowStep workflowStep, ITaskOperation operation)
    : TaskOperationFinishedEvent(operation)
{
    /// <summary>
    /// The workflow step that the operation is associated with.
    /// </summary>
    public WorkflowStep WorkflowStep { get; } = workflowStep;
}
